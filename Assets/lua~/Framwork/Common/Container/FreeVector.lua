---列表, 在遍历中可以删除元素，但是里面的元素不保证顺序，因为用了迭代器安全删除
---@class FreeVector
---@field private _size number
---@field private _it FreeVectorIt
---@field private _elms any[]
FreeVector =
DefineClass(
{
    _size = 0,
    _it = nil,
    _elms = nil
}
)

---创建一个free vec
---@return FreeVector
function FreeVector.S_NewFreeVector()
local ret = New(FreeVector)
ret._elms = {}
ret._it = FreeVectorIt.S_NewFreeVectorIt(ret)
return ret
end

---当前列表元素数量
---@return number
function FreeVector:Size()
return self._size
end

---列表是否为空
---@return boolean
function FreeVector:Empty()
return self._size == 0
end

---按索引访问元素
---@param i number 索引，从1开始
---@return any
function FreeVector:At(i)
return self._elms[i]
end

---计算给定元素在列表中的所谓位置
---@param e any 给定元素
---@return number 列表中的索引，没有找到返回0
function FreeVector:IndexOf(e)
for i = 1, self._size do
    if self._elms[i] == e then
        return i
    end
end
return 0
end

---开始遍历列表
---用法：
---it = myVec:Iter()
---while it:Valid() do
---   local elm = it:GetNext()
---   //do something to elm
---end
---@return FreeVectorIt 当前容器的一个遍历器（注意，这个遍历器是这个容器唯一的，同时只能有一个遍历过程存在）
function FreeVector:Iter()
self._it:SetIndex(1)
return self._it
end

---返回真正的元素数组，注意，外界不要随意更改
---@return any[]
function FreeVector:Elms()
return self._elms
end

---返回数值末尾元素
---@return any
function FreeVector:Back()
return self:At(self:Size())
end

---返回数值首元素
---@return any
function FreeVector:Front()
return self:At(1)
end

---在列表最后添加元素
---@param e any 带添加的元素
function FreeVector:PushBack(e)
self._size = self._size + 1
self._elms[self._size] = e
end

---取出列表最后一个元素
---@return any
function FreeVector:PopBack()
return self:RemoveAt(self._size)
end

---移除给定元素
---@param e any 待移除的元素
---@return any 被移除的元素，nil表示没有东西被移除
function FreeVector:Remove(e)
return self:RemoveAt(self:IndexOf(e))
end

---移除给定索引位置的元素
---@param idx number 索引位置
---@return any 被移除的元素，nil表示没有东西被移除
function FreeVector:RemoveAt(idx)
    local size = self._size
    local it = self._it
    local elms = self._elms

    if idx <= 0 or idx > size then
        -- idx无效
        return nil
    end

    local ret = elms[idx]
    if idx >= it:GetIndex() then
        -- idx在当前遍历器的后面，对遍历器没有影响
        elms[idx] = elms[size]
        elms[size] = nil
    else
        -- idx在遍历器之前，需要细致处理
        -- 首先在it之前的最后一个元素覆盖被删除元素位置
        elms[idx] = elms[it:GetIndex() - 1]
        -- 然后it位置的元素挪到它上一个位置
        elms[it:GetIndex() - 1] = elms[it:GetIndex()]
        -- 然后尾部元素挪到it位置
        elms[it:GetIndex()] = elms[size]
        -- 置空末尾元素
        elms[size] = nil
        -- 调整it的范围
        it:SetIndex(it:GetIndex() - 1)
    end
    -- 最后调整整个列表长度
    self._size = size - 1
    return ret
end

-- 查找If
---@param func fun(elm:any) :boolean 返回true表示满足条件
---@return any 查找的第一个满足条件的元素
function FreeVector:FindIf(func)
local i = 1
while i <= self._size do
    local elm = self:At(i)
    if func(elm) then
        return elm
    end
    i = i +1
end
return nil
end

---移除满足给定条件的元素
---@param func fun(elm:any) :boolean 返回true表示满足条件
---@param once boolean 是否只移除一次就返回，默认false，表示移除所有满足条件的元素
---@return any 最后一个被移除的元素
function FreeVector:RemoveIf(func, once)
local ret
local i = 1
while i <= self._size do
    local elm = self:At(i)
    if func(elm) then
        ret = elm
        self:RemoveAt(i)
        if once then
            break
        end
    else
        i = i + 1
    end
end
return ret
end

---移除满足给定条件的元素
---@param func fun(elm:any) :boolean 返回true表示满足条件
---@param once boolean 是否只移除一次就返回，默认false，表示移除所有满足条件的元素
---@return any 最后一个被移除的元素
function FreeVector:RemoveLastIf(func, once)
local ret
local i = self._size
while i >= 1 do
    local elm = self:At(i)
    if func(elm) then
        ret = elm
        self:RemoveAt(i)
        if once then
            break
        end
    end
    i = i - 1
end
return ret
end

---清理列表
function FreeVector:Clear()
self._elms = {}
self._size = 0
self._it:SetIndex(1)
end

---@class FreeVectorIt FreeVector的遍历器
---@field private _vec FreeVector
---@field private _index number
FreeVectorIt =
DefineClass(
{
    _vec = nil,
    _index = nil
}
)

---创建一个it
---@param vec FreeVector
---@return FreeVectorIt
function FreeVectorIt.S_NewFreeVectorIt(vec)
local ret = New(FreeVectorIt)
ret._vec = vec
ret._index = 1
return ret
end

---获取当前遍历器在Vec中的索引
---@return number
function FreeVectorIt:GetIndex()
return self._index
end

---设置index，遍历过程中不要调用
---@param index number
function FreeVectorIt:SetIndex(index)
self._index = index
end

---移动到下一个索引
function FreeVectorIt:MoveNext()
self._index = self._index + 1
end

---判断当前是否有效，索引是否超出
---@return boolean
function FreeVectorIt:Valid()
return self._index <= self._vec:Size()
end

---获取当前的元素
---@return any 当前元素，没有就返回nil
function FreeVectorIt:Element()
return self._vec:At(self._index)
end

---返回当前元素并移动到下一个元素
---@return any
function FreeVectorIt:GetNext()
local ret = self._vec:At(self._index)
self._index = self._index + 1
return ret
end

return FreeVector