---@class BaseProxy Proxy基类
BaseProxy = DeriveClass(IFrameworkMgr)

function BaseProxy:Init()
    self.data = {}
    self.listeners = {}
end

function BaseProxy:SetData(key, value)
    local oldValue = self.data[key]
    self.data[key] = value
    self:NotifyDataChange(key, value, oldValue)
end

function BaseProxy:GetData(key)
    return self.data[key]
end

---添加数据变化监听
---@param key string
---@param listener function
function BaseProxy:AddDataListener(key, listener)
    if not self.listeners[key] then
        self.listeners[key] = {}
    end
    table.insert(self.listeners[key], listener)
end

---通知数据变化
function BaseProxy:NotifyDataChange(key, newValue, oldValue)
    local listeners = self.listeners[key]
    if listeners then
        for _, listener in ipairs(listeners) do
            listener(newValue, oldValue)
        end
    end
end

function BaseProxy:Destroy()
    self.data = nil
    self.listeners = nil
end

return BaseProxy