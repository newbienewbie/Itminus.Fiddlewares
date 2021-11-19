module Tests

open System
open Xunit
open Itminus.Fiddlewares
open Itminus.Fiddlewares.Middleware

type Context = { DevMsg1 : int; DevMsg2 : string;  MstMsg: int;  Pending1 : int; Pending2: int}

[<Fact>]
let ``test always next`` () =

    let ctx: Context = {DevMsg1= 1; DevMsg2 ="a"; MstMsg = 1; Pending1 = 1; Pending2 = 2}

    let setPending1 p1 (ctx:Context) next =
        next {ctx with Pending1 = p1 }

    let setPending2 p2 (ctx:Context) next =
        next {ctx with Pending2 = p2 }
    
    let handle = 
        setPending1 3 
        >=> (setPending2 4)
        >=> (setPending1 -3)
        >=> (setPending2 -4)
    let final = fun ctx -> ctx

    let result = handle ctx final
    printfn "%A" result
    Assert.Equal(-3, result.Pending1)
    Assert.Equal(-4, result.Pending2)

    Assert.True(true)

[<Fact>]
let ``test conditional next`` () =

    let ctx: Context = {DevMsg1= 1; DevMsg2 ="a"; MstMsg = 1; Pending1 = 1; Pending2 = 2}

    let setPending1 predicate p1 (ctx:Context) next =
        if predicate ctx then
            next {ctx with Pending1 = p1 }
        else 
            ctx

    let setPending2 predicate p2 (ctx:Context) next =
        if predicate ctx then
            next {ctx with Pending2 = p2 }
        else
            ctx
    
    let handle = 
        setPending1 (fun ctx -> ctx.DevMsg1 = 1) 3 
        >=> (setPending2 (fun ctx -> ctx.DevMsg2 = "a") 4)
        >=> (setPending1 (fun ctx -> ctx.Pending1 = 3) -3)
        >=> (setPending2 (fun ctx -> ctx.Pending2 = 4) -4)
        >=> (setPending1 (fun ctx -> ctx.Pending1 = -30) -3)
        >=> (setPending2 (fun ctx -> ctx.Pending2 = -4) -40)
    let final = fun ctx -> ctx

    let result = handle ctx final
    printfn "%A" result
    Assert.Equal(-3, result.Pending1)
    Assert.Equal(-4, result.Pending2)

    Assert.True(true)

