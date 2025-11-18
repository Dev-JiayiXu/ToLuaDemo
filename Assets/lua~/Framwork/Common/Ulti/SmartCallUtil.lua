---@class SmartCallUtil 自动调用回调
SmartCallUtil = DefineClass()

---自动调用回调函数
---@param callback fun(arg1:any?)?  回调函数，如果为nil则不回调
---@param caller table? 被回调对象（可以传nil，说明是静态函数）
---@return any callback的返回值
function SmartCallUtil.SmartCall(callback, caller)
    if callback ~= nil then
        if caller ~= nil then
            return callback(caller)
        else
            return callback()
        end
    end
end

---自动调用回调函数
---@param callback fun(arg1:any, arg2:any?)? 回调函数，如果为nil则不回调
---@param simpleArg any 简单参数，可选
---@param caller table? 被回调对象（可以传nil，说明是静态函数）
---@return any callback的返回值
function SmartCallUtil.SmartCall1(callback, simpleArg, caller)
    if callback ~= nil then
        if caller ~= nil then
            return callback(caller, simpleArg)
        else
            return callback(simpleArg)
        end
    end
end

---自动调用回调函数
---@param callback  fun(arg1:any, arg2:any, arg3:any?)? 回调函数，如果为nil则不回调
---@param simpleArg1 any 简单参数1，可选
---@param simpleArg2 any 简单参数2，可选
---@param caller table? 被回调对象（可以传nil，说明是静态函数）
---@return any callback的返回值
function SmartCallUtil.SmartCall2(callback, simpleArg1, simpleArg2, caller)
    if callback ~= nil then
        if caller ~= nil then
            return callback(caller, simpleArg1, simpleArg2)
        else
            return callback(simpleArg1, simpleArg2)
        end
    end
end

---自动调用回调函数
---@param callback fun(arg1:any, arg2:any, arg3:any, arg4:any?)? 回调函数，如果为nil则不回调
---@param simpleArg1 any 简单参数1，可选
---@param simpleArg2 any 简单参数2，可选
---@param simpleArg3 any 简单参数2，可选
---@param caller table? 被回调对象（可以传nil，说明是静态函数）
---@return any callback的返回值
function SmartCallUtil.SmartCall3(callback, simpleArg1, simpleArg2, simpleArg3, caller)
    if callback ~= nil then
        if caller ~= nil then
            return callback(caller, simpleArg1, simpleArg2, simpleArg3)
        else
            return callback(simpleArg1, simpleArg2, simpleArg3)
        end
    end
end

---自动调用回调函数
---@param callback fun(...)? 回调函数，如果为nil则不回调
---@param caller table? 被回调对象（可以传nil，说明是静态函数）
---@vararg any[] | any 不定参数
---@return any callback的返回值
function SmartCallUtil.SmartCallWithArgs(callback, caller, ...)
    if callback ~= nil then
        if caller ~= nil then
            return callback(caller, ...)
        else
            return callback(...)
        end
    end
end

---自动调用回调函数
---@param callback fun(arg:any, ...)? 回调函数，如果为nil则不回调
---@param caller table? 被回调对象（可以传nil，说明是静态函数）
---@param simpleArg any 额外参数，可选
---@vararg any[] | any 不定参数
---@return any callback的返回值
function SmartCallUtil.SmartCallWithArgs2(callback, caller, simpleArg, ...)
    if callback ~= nil then
        if caller ~= nil then
            return callback(caller, simpleArg, ...)
        else
            return callback(simpleArg, ...)
        end
    end
end

---自动调用回调函数
---@param callback fun(...)? 回调函数，如果为nil则不回调
---@param caller table? 被回调对象（可以传nil，说明是静态函数）
---@param args any[] pack完毕的不定参数
---@return any callback的返回值
function SmartCallUtil.SmartApplyWithArgs(callback, caller, args)
    if callback ~= nil then
        if caller ~= nil then
            return callback(caller, args)
        else
            return callback(args)
        end
    end
end

---自动调用回调函数
---@param callback fun(arg:any, ...) 回调函数，如果为nil则不回调
---@param caller table? 被回调对象（可以传nil，说明是静态函数）
---@param simpleArg any 额外参数
---@param args any[] pack完毕的不定参数
---@return any callback的返回值
function SmartCallUtil.SmartApplyWithArgs2(callback, caller, simpleArg, args)
    if callback ~= nil then
        if caller ~= nil then
            return callback(caller, simpleArg, args)
        else
            return callback(simpleArg, args)
        end
    end
end

return SmartCallUtil
