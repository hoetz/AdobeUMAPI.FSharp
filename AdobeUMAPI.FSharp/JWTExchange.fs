module JWTExchange

open System.Security.Cryptography.X509Certificates
open System
open JWT.Builder
open JWT.Algorithms
open FSharp.Data

let ims_host = "ims-na1.adobelogin.com"
let ims_endpoint_jwt = "/ims/exchange/jwt"
let umapi_host = "usermanagement.adobe.io"
let umapi_endpoint = "/v2/usermanagement"

type JwtAccessTokenResponse = JsonProvider<"""{
    "token_type": "bearer",
    "access_token": "sampleToken",
    "expires_in": 86399992
}""">

type GetUserInfoResponse = JsonProvider<"""{
  "result": "success",
  "user": {
    "email": "jdoe@my-domain.com",
    "status": "active",
    "groups": [
      "UserGroup1",
      "UserGroup2"
    ],
    "username": "jdoe@my-domain.com",
    "domain": "my-domain.com",
    "country": "JP",
    "type": "enterpriseID"
  }
}""">

type adobeApiOptions= {
        priv_key_file_path:string;
        priv_key_file_password:string;
        org_id:string;
        tech_acct:string;
        client_id:string;
        client_secret:string
        }

let createJwtBase64 (apiOptions:adobeApiOptions)=
    let expiry_time=DateTimeOffset.UtcNow.AddHours(24.).ToUnixTimeSeconds()
    let aud=sprintf "https://%s/c/%s" ims_host apiOptions.client_id
    let privKeyCert=new X509Certificate2(apiOptions.priv_key_file_path,apiOptions.priv_key_file_password)
    let rs265=new RS256Algorithm(privKeyCert)
    let scope="ent_user_sdk"

    let builder = new JwtBuilder()
    let jwt=builder.WithAlgorithm(rs265)
                      .WithSecret(apiOptions.client_secret)
                      .AddClaim("exp", expiry_time)
                      .AddClaim("iss", apiOptions.org_id)
                      .AddClaim("sub", apiOptions.tech_acct)
                      .AddClaim("aud", aud)
                      .AddClaim(sprintf "https://%s/s/%s" ims_host scope, true)
                      .Build()
    jwt.ToString()

let createAccessToken (apiOptions:adobeApiOptions) (jwt:string)=
    let url=sprintf "https://%s%s"ims_host ims_endpoint_jwt
    let response=Http.Request(
                    url,
                    headers=[|("Content-Type","application/x-www-form-urlencoded");
                              ("Cache-Control","no-cache")|] ,
                    body = FormValues [|
                                ("client_id", apiOptions.client_id);
                                ("client_secret", apiOptions.client_secret);
                                ("jwt_token", jwt)
                                |])
    if response.StatusCode=200 then
        let jsonResp=response.Body
        match jsonResp with
        | Text txt -> JwtAccessTokenResponse.Parse(txt).AccessToken
        | Binary _ -> failwith (sprintf "Invalid Adobe Jwt Access token response,
                        expected json txt %s" (response.ToString()))
    else
        failwith (sprintf "Invalid Adobe Jwt Access token response %s" (response.ToString()))