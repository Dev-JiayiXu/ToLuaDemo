---@class BaseView:IFrameworkMgr View基类
BaseView = DeriveClass(IFrameworkMgr)

function BaseView:Init()
    self.components = {}
    self.proxy = nil
    self.isActive = false
end

function BaseView:OnStart()
end

function BaseView:OnUpdate()
end

function BaseView:OnShow()
    self.isActive = true
end

function BaseView:OnHide()
    self.isActive = false
end

function BaseView:OnRefresh()
end

function BaseView:OnDestroy()
    self.components = nil
    self.proxy = nil
end

---获取组件
---@param name string
---@return any 组件
function BaseView:GetComponent(name)
    return self.components and self.components[name]
end

---设置Proxy
---@param proxy BaseProxy
function BaseView:SetProxy(proxy)
    self.proxy = proxy
end

---按钮事件
---@param buttonName string 按钮字段名
---@param handler function 处理函数
function BaseView:BindButton(buttonName, handler)
    local button = self:GetComponent(buttonName)
    if button and button.onClick then
        button.onClick:AddListener(handler)
    end
end
return BaseView