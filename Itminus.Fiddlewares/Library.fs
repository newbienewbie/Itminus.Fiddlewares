namespace Itminus.Fiddlewares

/// ctx * next -> _
type Middleware<'Context,'Result> = ('Context -> 'Result) -> 'Result

module Middleware = 
    /// 运行一个中间件函数：需要传递一个延续子
    let run mw (cont: 'Context -> 'Result) = 
        mw cont

    let bind fn mw1 = 
        // 合成 mw1 与 mw2 之后的新中间件特征：
        //     1. 合成后的中间件其必然也接收一个next函数作为延续，记作 next 。
        //     2. 管道第一个必然还是mw1，mw1的延续(cont1)的作用是在合适的时间点触发后续的中间件(mw2)的调用
        //     3. 管道第二个中间件是mw2，由fn生成。mw2的延续(cont2)等同于最终为整个最终的延续next。
        fun next ->
            let cont1 ctx =                     // mw1的延续: 会触发mw2的调用
                let mw2 = fn ctx                // 由fn生成后续的中间件 mw2
                run mw2 next                    // cont2 等同于 合成mw'的延续cont'
            run mw1 cont1

    /// bind operator
    let (>>=) m fn = bind fn m

    /// 复合两个 'ctx -> 'next -> 'result 函数
    let (>=>) f1 f2 =  f1 >> bind f2
