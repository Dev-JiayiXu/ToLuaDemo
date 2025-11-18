---@class EventContainerInfo
---@field dispatcher EventDispatcher
---@field evtName number
---@field func function
---@field funcObj any?

---@class EventContainer 事件容器
---@field private _eventList EventContainerInfo[]
EventContainer = DefineClass()

function EventContainer.S_NewEventContainer()
    local eventContainer = New(EventContainer)
    eventContainer._eventList = {}
    return eventContainer
end

---添加一个事件
---@param dispatcher EventDispatcher 事件的派发者
---@param evtName number 事件名称
---@param func function 待添加的函数名
---@param funcObj any? 被回调对象（可以传nil，说明是静态函数）
function EventContainer:AddEvent(dispatcher, evtName, func, funcObj)
    self._eventList[#self._eventList + 1] = {dispatcher = dispatcher, evtName = evtName, func = func, funcObj = funcObj}
    dispatcher:AddEventHandler(evtName, func, funcObj)
end

--- 移除一个事件
---@param dispatcher EventDispatcher 事件的派发者
---@param evtName number 事件名称
---@param func function? 待删除的函数名，填nil代表清除该事件名下的所有函数
---@param funcObj any? 被回调对象（可以传nil，说明是静态函数）
function EventContainer:RemoveEvent(dispatcher, evtName, func, funcObj)
    local isRemove = false
    local i = 1
    while i <= #self._eventList do
        local e = self._eventList[i]
        if dispatcher == e.dispatcher and (not evtName or evtName == e.evtName) and (not func or func == e.func) and (not funcObj or funcObj == e.funcObj) then
            e.dispatcher:RemoveEventHandler(evtName, func, funcObj)
            isRemove = true
            table.remove(self._eventList, i)
        else
            i = i + 1
        end
    end
    if not isRemove and evtName and func then
        dispatcher:RemoveEventHandler(evtName, func, funcObj)
    end
end

--- 清除所有事件
function EventContainer:RemoveAllEvent()
    for i = #self._eventList, 1, -1 do
        local e = self._eventList[i]
        e.dispatcher:RemoveEventHandler(e.evtName, e.func, e.funcObj)
    end
    self._eventList = nil
end

return EventContainer