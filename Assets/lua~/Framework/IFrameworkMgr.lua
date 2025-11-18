---@class IFrameworkMgr:EventDispatcher  IFrameworkMgr继承自EventDispatcher
IFrameworkMgr = DeriveClass(EventDispatcher)

---初始化
function IFrameworkMgr:Init()
end

---重连
function IFrameworkMgr:Reconnect()
end

---销毁
function IFrameworkMgr:Destroy()
end

return IFrameworkMgr