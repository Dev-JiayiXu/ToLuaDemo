---@class MainView:BaseView 主界面视图
MainView = DeriveClass(BaseView)

function MainView:OnStart()
    print("MainView启动")
    
    -- 获取组件
    self.openBagBtn = self:GetComponent("openBagBtn")
    
    -- 绑定按钮事件
    if self.openBagBtn then
        self:BindButton("openBagBtn", function()
            self:OnOpenBagClick()
        end)
        print("找到打开背包按钮并绑定事件")
    else
        print("警告: 未找到openBagBtn组件")
    end
end

function MainView:OnOpenBagClick()
    print("点击打开背包按钮")
    
    -- 创建并显示背包界面
    self:OpenBagView()
end

function MainView:OpenBagView()
    -- 动态加载BagView和BagProxy
    local success, bagViewClass = pcall(require, "BagView")
    local success2, bagProxyClass = pcall(require, "BagProxy")
    
    if success and success2 then
        -- 创建背包代理
        local bagProxy = bagProxyClass.New()
        bagProxy:Init()
        
        -- 创建背包视图
        local bagView = bagViewClass.New()
        bagView:SetProxy(bagProxy)
        
        -- 这里可以设置背包视图的父节点等
        -- 由于是动态创建的，你可能需要手动设置gameObject和transform
        -- 或者通过UIManager来管理
        
        print("背包界面创建成功")
    else
        print("加载背包相关模块失败")
    end
end

function MainView:OnDestroy()
    print("MainView销毁")
end

return MainView