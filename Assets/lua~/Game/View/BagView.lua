---@class BagView:BaseView 背包界面视图
BagView = DeriveClass(BaseView)

function BagView:OnStart()
    print("BagView动态启动")
    
    -- 设置物品预制体名称
    self.bagItemPrefabName = "BagItem"
    
    -- 如果已经有代理，初始化UI
    if self.proxy then
        self:InitializeUI()
    else
        print("BagView: 等待设置数据代理")
    end
end

function BagView:SetProxy(proxy)
    BaseView.SetProxy(self, proxy)
    
    -- 如果已经启动，初始化UI
    if self.proxy then
        self:InitializeUI()
    end
end

function BagView:InitializeUI()
    print("BagView初始化UI")
    
    -- 监听数据变化
    self.proxy:AddDataListener("BagItemList", function(newValue, oldValue)
        self:OnBagDataChanged(newValue, oldValue)
    end)
    
    -- 初始刷新UI
    self:RefreshBagItems()
end

function BagView:RefreshBagItems()
    if not self.proxy then
        print("警告: 没有设置数据代理")
        return
    end
    
    -- 这里需要有一个content组件来放置物品
    -- 如果是动态创建的，可能需要通过其他方式获取content
    if not self.content then
        print("警告: 没有content组件，无法显示物品")
        return
    end
    
    -- 清空现有物品
    self:ClearBagItems()
    
    -- 获取物品数量
    local itemCount = self.proxy:GetBagItemCount()
    print("需要创建 " .. itemCount .. " 个背包物品")
    
    -- 创建物品
    for i = 1, itemCount do
        self:CreateBagItem(i)
    end
end

-- 其他方法保持不变...

function BagView:ClearBagItems()
    if not self.content then return end
    
    local childCount = self.content.transform.childCount
    for i = childCount - 1, 0, -1 do
        local child = self.content.transform:GetChild(i)
        if child and child.gameObject then
            ResourceManager:DestroyInstance(child.gameObject)
        end
    end
end

function BagView:CreateBagItem(index)
    if not self.content then return end
    
    local bagItem = ResourceManager:InstantiateAsync(
        self.bagItemPrefabName, 
        Vector3.zero, 
        Quaternion.identity
    )

    -- 设置父节点为content
    if bagItem then
        bagItem.transform:SetParent(self.content.transform, false)
        self:SetupBagItemUI(bagItem, index)
    else
        print("错误: 实例化背包物品失败 " .. self.bagItemPrefabName)
    end
end

function BagView:SetupBagItemUI(bagItem, index)
    local nameText = bagItem.transform:Find("NameText")
    if nameText then
        local textComponent = nameText:GetComponent("Text")
        if textComponent then
            textComponent.text = "物品 " .. index
        end
    end
    
    local iconImage = bagItem.transform:Find("IconImage")
    if iconImage then
        local imageComponent = iconImage:GetComponent("Image")
        if imageComponent then
            -- 在这设置图标
        end
    end
end

function BagView:OnDataChanged(key, newValue, oldValue)
    if key == "BagItemList" then
        print("背包数据变化，刷新UI")
        self:RefreshBagItems()
    end
end

function BagView:OnDestroy()
    print("BagView销毁")
    self:ClearBagItems()
end

return BagView