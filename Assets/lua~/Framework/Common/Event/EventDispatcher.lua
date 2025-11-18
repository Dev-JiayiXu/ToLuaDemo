---@class EventDispatcher 事件派发器
---@field private _eventHanders table<number, Delegate> 
EventDispatcher = DefineClass()

---添加一个事件回调
---@param evtName number 事件名称
---@param func fun(...) 回调函数，函数签名由具体功能决定
---@param thisObj table? 回调函数的self，可选
function EventDispatcher:AddEventHandler(evtName, func, thisObj)
    if evtName == nil or func == nil then
        Debug.LogError("need a function and evt for handle event...")
        return
    end
    -- 若事件或函数不为空，创建一个函数回调
    if self._eventHanders == nil then
        self._eventHanders = {}
    end
    local h = self._eventHanders[evtName]
    if not h then
        h = Delegate.S_NewDelegate()
        self._eventHanders[evtName] = h
    end
    h:AddCallback(func, thisObj)
end

---移除一个事件回调
---@param evtName number 事件名称
---@param func fun(...)? 待移除的回调函数，如果填nil，则表示thisObj注册的所有函数都清理
---@param thisObj table? 回调函数的self，可选
function EventDispatcher:RemoveEventHandler(evtName, func, thisObj)
    if evtName == nil or (thisObj == nil and func == nil) then
        print("evt and (listener or func) can't all be nil'")
        return
    end
    if self._eventHanders then
        local h = self._eventHanders[evtName]
        if h then
            h:RemoveCallback(func, thisObj)
        end
    end
end

---移除给定对象注册的所有事件回调
---@param thisObj table
function EventDispatcher:RemoveEvents(thisObj)
    if self._eventHanders then
        for _, h in pairs(self._eventHanders) do
            h:RemoveCallback(nil, thisObj)
        end
    end
end

---清理所有事件注册
function EventDispatcher:ClearEvents()
    self._eventHanders = {}
end

---触发给定事件
---@param evtName number 事件名称
---@param simpleArg any? 任一一个参数
function EventDispatcher:InvokeEvent(evtName, simpleArg)
    if self._eventHanders then
        local h = self._eventHanders[evtName]
        if h then
            h:Invoke(simpleArg)
        end
    end
end

---触发给定事件
---@param evtName number 事件名称
---@vararg any[] | any 支持不定参数
function EventDispatcher:InvokeEventWithArgs(evtName, ...)
    if self._eventHanders then
        local h = self._eventHanders[evtName]
        if h then
            h:InvokeWithArgs(...)
        end
    end
end

---判断给定事件是否有监听者注册
---@param evtName number 事件名称
---@return boolean 是否有监听者注册
function EventDispatcher:WillListenEvent(evtName)
    if self._eventHanders then
        local h = self._eventHanders[evtName]
        if h then
            return h:Size() > 0
        end
    end
    return false
end

return EventDispatcher