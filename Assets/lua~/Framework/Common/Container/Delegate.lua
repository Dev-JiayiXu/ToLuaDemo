---@class Delegate 回调委托
---@field private _listeners FreeVector 监听者列表
Delegate = DefineClass()

---@return Delegate
function Delegate.S_NewDelegate()
    local ret = New(Delegate)
    return ret
end

---清理所有监听者
function Delegate:Clear()
    if self._listeners then
        self._listeners:Clear()
    end
end

---获取监听者数量
---@return number
function Delegate:Size()
    return self._listeners and self._listeners:Size() or 0
end

---添加一个回调
---@param func fun(...) 回调函数
---@param thisObj table? 回调函数的self，可选
function Delegate:AddCallback(func, thisObj)
    if func == nil then
        Debug.Log("need a function for delegate...")
        return
    end
    if not self._listeners then
        self._listeners = FreeVector.S_NewFreeVector()
    end
    self._listeners:PushBack({func = func, thisObj = thisObj})
end

---移除一个回调
---@param func fun(...)? 待移除的回调函数，如果填nil，则表示thisObj注册的所有函数都清理
---@param thisObj table? 回调函数的self，可选
function Delegate:RemoveCallback(func, thisObj)
    if thisObj == nil and func == nil then
        Debug.Log("thisObj or func can't all be nil'")
        return
    end
    if self._listeners then
        self._listeners:RemoveIf(
            function(ls)
                return (thisObj == ls.thisObj) and (func == nil or func == ls.func)
            end,
            func ~= nil
        )
    end
end

---触发委托
---@param oneArgOrNil any 任一一个参数，可选
function Delegate:Invoke(oneArgOrNil)
    if self._listeners then
        local it = self._listeners:Iter()
        while it:Valid() do
            local ls = it:GetNext()
            SmartCallUtil.SmartCall1(ls.func, oneArgOrNil,ls.thisObj)
        end
    end
end

---触发委托
---@vararg any[] | any 支持不定参数
function Delegate:InvokeWithArgs(...)
    if self._listeners then
        local it = self._listeners:Iter()
        while it:Valid() do
            local ls = it:GetNext()
            SmartCallUtil.SmartCallWithArgs(ls.func, ls.thisObj, ...)
        end
    end
end

---触发委托并清理所有注册
---@param oneArgOrNil any 任一一个参数，可选
function Delegate:InvokeOnce(oneArgOrNil)
    if self._listeners then
        local listeners = self._listeners
        self._listeners = nil
        local it = listeners:Iter()
        while it:Valid() do
            local ls = it:GetNext()
            SmartCallUtil.SmartCall1(ls.func, oneArgOrNil,ls.thisObj)
        end
        listeners:Clear()
        if not self._listeners then
            self._listeners = listeners
        end
    end
end

---触发委托并清理所有注册
---@vararg any[] | any 支持不定参数
function Delegate:InvokeOnceWithArgs(...)
    if self._listeners then
        local ls = self._listeners
        self._listeners = nil
        local it = self._listeners:Iter()
        while it:Valid() do
            local ls = it:GetNext()
            SmartCallUtil.SmartCallWithArgs(ls.func,ls.thisObj,  ...)
        end
        ls:Clear()
        if not self._listeners then
            self._listeners = ls
        end
    end
end

return Delegate