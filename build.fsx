#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Target //"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

Target.initEnvironment ()

Target.create "Clean" (fun _ ->
    !! "Itminus.Fiddlewares/**/bin"
    ++ "Itminus.Fiddlewares/**/obj"
    !! "Itminus.Fiddlewares.Tests/**/bin"
    ++ "Itminus.Fiddlewares.Tests/**/obj"
    |> Shell.cleanDirs 
)

Target.create "Build" (fun _ ->
    !! "Itminus.Fiddlewares/**/*.*proj"
    |> Seq.iter (DotNet.build id)
)

Target.create "Test" (fun _ ->
    DotNet.test id "Itminus.Fiddlewares.Tests/Itminus.Fiddlewares.Tests.fsproj"
)

Target.create "Pack" (fun _ ->
    DotNet.pack id "Itminus.Fiddlewares/Itminus.Fiddlewares.fsproj"
)

Target.create "All" ignore

"Clean"
  ==> "Build"
  ==> "Test"
  ==> "Pack"
  ==> "All"

Target.runOrDefault "All"