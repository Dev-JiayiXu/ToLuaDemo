---@class BagView:BaseView 背包View
BagView = DeriveClass(BaseView)

function BagView:OnStart()
    print("BagView启动")
    
    self.content = self:GetComponent("content")
    if not self.content then
        print("错误: 未找到content组件")
        return
    end
    
    -- 设置物品预制体名称
    self.bagItemPrefabName = "BagItem"
    self:RefreshBagItems()
end

function BagView:OnUpdate()
end

function BagView:RefreshBagItems()
    if not self.proxy then
        print("错误: 没有Proxy")
        return
    end
    
    self:ClearBagItems()
    local itemCount = self.proxy:GetBagItemCount()
    for i = 1, itemCount do
        self:CreateBagItem(i)
    end
end

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