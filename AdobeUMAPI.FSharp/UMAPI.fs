module UMAPI
open JWTExchange
open FSharp.Data

let getUserInformation (apiOptions:adobeApiOptions) (accessToken:string )(upn:string)=
    let url=sprintf "https://%s/v2/usermanagement/organizations/%s/users/%s" umapi_host apiOptions.org_id upn
    let resp=Http.Request(
                            url,
                            headers=[|("X-Api-Key",apiOptions.client_id);
                                ("Authorization",sprintf "Bearer %s" accessToken);
                                ("content-type","application/json")|],
                            silentHttpErrors=true
                            )
    match resp.StatusCode with
    | 200 ->
        match resp.Body with
        | Text txt -> Some (GetUserInfoResponse.Parse(txt).User)
        | _ -> failwith "Invalid Adobe user info response"
    | 404 -> None
    | _ -> failwith (sprintf "Invalid user info response from Adobe API: %s" (resp.StatusCode.ToString()))