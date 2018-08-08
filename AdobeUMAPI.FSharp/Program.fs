open System
open FSharp.Configuration
open JWTExchange
open UMAPI

type ApiConfig = YamlConfig<"SecretConfig.yaml", InferTypesFromStrings = false>

[<EntryPoint>]
let main argv =
    let apiConfig= new ApiConfig()
    let apiOptions={
        org_id=apiConfig.org_id;
        tech_acct=apiConfig.tech_acct;
        client_id=apiConfig.api_key.ToString();
        client_secret=apiConfig.client_secret.ToString();
        priv_key_file_path=apiConfig.cert_filename;
        priv_key_file_password=apiConfig.cert_password
    }
    let jwt=createJwtBase64 apiOptions
    let accessToken=createAccessToken apiOptions jwt

    let userInfo=getUserInformation apiOptions accessToken "john.doe@yourdomain.com"
    match userInfo with
    | Some x -> printfn "%s found!" x.Username
    | None -> printf "User not found"

    0 // return an integer exit code