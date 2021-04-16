open System

let help _ = 
    let output = """SqliteToMysql
Usage:
    SqliteToMysql <input_sqlite_file_path> <output_mysql_file_path>
"""
    eprintfn "%s" output


let rec remove (str: String) (list: (string*string) list) = 
    match list with
    | [] -> str
    | (key, value)::tail -> remove (str.Replace(key, value)) tail 
    

[<EntryPoint>]
let main argv =
    
    match argv.Length with
    | 2 -> 
        match IO.File.Exists(argv.[0]) with
        | true ->
            use stream_reader = IO.File.OpenText(argv.[0])
            let content = "SET FOREIGN_KEY_CHECKS=0;" + stream_reader.ReadToEnd()

            printfn "读取%s" argv.[0]

            let need_replace = [
                ("BEGIN TRANSACTION;", "")
                ("COMMIT;", "")
                ("IF NOT EXISTS", "")
                ("AUTOINCREMENT", "")
                ("\"", "`")
                
                ("INTEGER", "int(11)")
                
                ("TEXT", "longtext")
                ("UNIQUE", "")
            ]

            let content = remove content need_replace

            printfn "%A" content

            use stream_writter = IO.File.CreateText(argv.[1])
            stream_writter.Write(Text.StringBuilder(content))
            printfn "转换完成,文件已存储至%s" argv.[1]
            
        | false ->
            eprintfn "无法找到文件 %A" argv.[0]
    | _ -> 
        help()
    0 
    