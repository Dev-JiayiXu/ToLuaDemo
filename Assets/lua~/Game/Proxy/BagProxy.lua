---@class BagProxy:BaseProxy 背包Proxy
BagProxy = DeriveClass(BaseProxy)

function BagProxy:Init()
    BaseProxy.Init(self)
    
    -- 初始化背包物品列表，我这里就是测试玩玩，正常应该从服务器中获取到数据
    self.BagItemList = {
        {id = 1, name = "生命药水", count = 3},
        {id = 2, name = "魔法药水", count = 5},
        {id = 3, name = "长剑", count = 1},
        {id = 4, name = "盾牌", count = 1},
        {id = 5, name = "弓箭", count = 10}
    }
end

---获取背包物品数量
---@return number
function BagProxy:GetBagItemCount()
    return #self.BagItemList
end

---获取背包物品
---@param index number
---@return table 物品数据
function BagProxy:GetBagItem(index)
    if index >= 1 and index <= #self.BagItemList then
        return self.BagItemList[index]
    end
    return nil
end

---添加物品到背包
---@param item table
function BagProxy:AddBagItem(item)
    if not item then return end
    
    table.insert(self.BagItemList, item)
    self:NotifyDataChange("BagItemList", self.BagItemList, nil)
end

---移除最后一个物品
function BagProxy:RemoveLastBagItem()
    if #self.BagItemList > 0 then
        local removedItem = table.remove(self.BagItemList)
        self:NotifyDataChange("BagItemList", self.BagItemList, nil)
    end
end

---更新物品数量
---@param index number
---@param newCount number
function BagProxy:UpdateItemCount(index, newCount)
    if index >= 1 and index <= #self.BagItemList then
        local oldCount = self.BagItemList[index].count
        self.BagItemList[index].count = newCount
        self:NotifyDataChange("itemCount_" .. index, newCount, oldCount)
    end
end

function BagProxy:Destroy()
    self.BagItemList = nil
    BaseProxy.Destroy(self)
end

return BagProxy