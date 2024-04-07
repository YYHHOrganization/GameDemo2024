using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class YRouge_CorridorNode : YRouge_Node
{
    YRouge_Node structure1;
    YRouge_Node structure2;
    int corridorWidth;
    private int modifierDistanceFromWall = 1;
    public YRouge_CorridorNode(YRouge_Node structure1, YRouge_Node structure2, int corridorWidth) : base(null)
    {
        this.structure1 = structure1;
        this.structure2 = structure2;
        this.corridorWidth = corridorWidth;
        GenerateCorridor();
        
    }

    private void GenerateCorridor()
    {
        var relativePositionOfStructure2 = CheckPositionStructure1AgainstStructure2();
        switch(relativePositionOfStructure2)
        {
            case RelativePosition.Up:
                ProcessRoomInRelativePositionUpOrDown(this.structure1, this.structure2);
                break;
            case RelativePosition.Down:
                ProcessRoomInRelativePositionUpOrDown(this.structure2, this.structure1);
                break;
            case RelativePosition.Right:
                ProcessRoomInRelativePositionRightOrLeft(this.structure1, this.structure2);
                break;
            case RelativePosition.Left:
                ProcessRoomInRelativePositionRightOrLeft(this.structure2, this.structure1);
                break;
        }
    }

    private void ProcessRoomInRelativePositionRightOrLeft(YRouge_Node p0, YRouge_Node p1)
    {
        YRouge_Node leftStructure = null;
        List<YRouge_Node> leftStructureChildren = YRouge_StructureHelper.TraverseGraphToExtractLowestLeafs(structure1);
        YRouge_Node rightStructure = null;
        List<YRouge_Node> rightStructureChildren = YRouge_StructureHelper.TraverseGraphToExtractLowestLeafs(structure2);
        
        //------开始看靠左的------
        //表示左边的结构
        var sortedLeftStructure = leftStructureChildren.OrderByDescending(child => child.TopRightAreaCorner.x).ToList();
        //如果只有一个结构，那么就直接选取
        if (sortedLeftStructure.Count == 1)
        {
            leftStructure = sortedLeftStructure[0];
        }
        //如果有多个结构，那么就选取最右边的结构
        else
        {
            //这个maxX是左边结构中最右边的结构的x轴坐标
            int maxX = sortedLeftStructure[0].TopRightAreaCorner.x;
            //取出x轴比较右的结构，同时跟最左边的比较，如果太左了直接不放进List。
            sortedLeftStructure = sortedLeftStructure.
                Where(children => Mathf.Abs(maxX - children.TopRightAreaCorner.x) < 10).ToList();
            //此处index随机选择一个比较右的结构
            int index = UnityEngine.Random.Range(0, sortedLeftStructure.Count);
            leftStructure = sortedLeftStructure[index];//如果这个不符合要求，那么后面可以在List中换一个
        }
        
        //------开始看靠右的------
        //随便选个靠右的左边的，看看右边的与没有匹配的
        //表示右边的结构，找到右边结构中，与左边结构有可能相邻的结构
        var possibleNeighboursInRightStructureList = rightStructureChildren.Where(
            child => GetValidYForNeighourLeftRight(
                leftStructure.TopRightAreaCorner,//左边结构的右上角
                leftStructure.BottomRightAreaCorner,//左边结构的右下角
                child.TopLeftAreaCorner,//右边结构的左上角
                child.BottomLeftAreaCorner//右边结构的左下角
            ) != -1
        ).OrderBy(child => child.BottomRightAreaCorner.x).ToList();//如果有可能相邻，那么就按照x轴从小到大排序，找到最左边的结构
        
        //选个最匹配的右边的结构
        //如果没有可能相邻的结构，那么就直接选取最左边的结构
        if (possibleNeighboursInRightStructureList.Count <= 0)
        {
            rightStructure = structure2;
        }
        else
        {
            rightStructure = possibleNeighboursInRightStructureList[0];
        }
        
        //此时我们并不知道二者是否匹配
        //此处是在左边结构中找到一个y值，使得左右两个结构相交？
        int y = GetValidYForNeighourLeftRight(
            leftStructure.TopLeftAreaCorner, //左边结构的左上角
            leftStructure.BottomRightAreaCorner,//左边结构的右下角 ？？？？ 不过只比较y 所以也无所谓吧
            rightStructure.TopLeftAreaCorner,//右边结构的左上角
            rightStructure.BottomLeftAreaCorner);//右边结构的左下角
        //如果没有找到合适的y值，那么就继续找  y==-1不匹配
        while(y==-1 && sortedLeftStructure.Count > 1)
        {
            //删掉之前测试过的结构
            sortedLeftStructure = sortedLeftStructure.Where(
                child => child.TopLeftAreaCorner.y != leftStructure.TopLeftAreaCorner.y).ToList();
            leftStructure = sortedLeftStructure[0];
            y = GetValidYForNeighourLeftRight(leftStructure.TopLeftAreaCorner, leftStructure.BottomRightAreaCorner,
                rightStructure.TopLeftAreaCorner,
                rightStructure.BottomLeftAreaCorner);
        }
        //最后找到了合适的y值，那么就可以生成走廊了
        BottomLeftAreaCorner = new Vector2Int(leftStructure.BottomRightAreaCorner.x, y);
        TopRightAreaCorner = new Vector2Int(rightStructure.TopLeftAreaCorner.x, y + this.corridorWidth);
    }

    /// <summary>
    /// 以下方法用于计算左右两个结构之间的有效Y值，见笔记图1111  //我的理解就是计算两个结构的相交处，取中心点
    /// </summary>
    /// <param name="leftNodeUp"></param>
    /// <param name="leftNodeDown"></param>
    /// <param name="rightNodeUp"></param>
    /// <param name="rightNodeDown"></param>
    /// <returns>返回的是两个结构相交处的middle值</returns>
    private int GetValidYForNeighourLeftRight(Vector2Int leftNodeUp, Vector2Int leftNodeDown, Vector2Int rightNodeUp, Vector2Int rightNodeDown)
    {
       
        //右上>左上，左下>右下 左边的全被右边的包含 二者相交处就是左边
        if(rightNodeUp.y >= leftNodeUp.y && leftNodeDown.y >= rightNodeDown.y)
        {
            return YRouge_StructureHelper.CalculateMiddlePoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
            ).y;
        }
        if(rightNodeUp.y <= leftNodeUp.y && leftNodeDown.y <= rightNodeDown.y)
        {
            return YRouge_StructureHelper.CalculateMiddlePoint(
                rightNodeDown+new Vector2Int(0,modifierDistanceFromWall),
                rightNodeUp - new Vector2Int(0, modifierDistanceFromWall+this.corridorWidth)
            ).y;
        }
        if(leftNodeUp.y >= rightNodeDown.y && leftNodeUp.y <= rightNodeUp.y)
        {
            return YRouge_StructureHelper.CalculateMiddlePoint(
                rightNodeDown+new Vector2Int(0,modifierDistanceFromWall),
                leftNodeUp-new Vector2Int(0,modifierDistanceFromWall)
            ).y;
        }
        if(leftNodeDown.y >= rightNodeDown.y && leftNodeDown.y <= rightNodeUp.y)
        {
            return YRouge_StructureHelper.CalculateMiddlePoint(
                leftNodeDown+new Vector2Int(0,modifierDistanceFromWall),
                rightNodeUp-new Vector2Int(0,modifierDistanceFromWall+this.corridorWidth)
            ).y;
        }
        return- 1;//二者完全不相交 错开了
    }
    
    
    private void ProcessRoomInRelativePositionUpOrDown(YRouge_Node structure1, YRouge_Node structure2)
    {
        YRouge_Node bottomStructure = null;
        List<YRouge_Node> structureBottmChildren = YRouge_StructureHelper.TraverseGraphToExtractLowestLeafs(structure1);
        YRouge_Node topStructure = null;
        List<YRouge_Node> structureAboveChildren = YRouge_StructureHelper.TraverseGraphToExtractLowestLeafs(structure2);

        var sortedBottomStructure = structureBottmChildren.OrderByDescending(child => child.TopRightAreaCorner.y).ToList();

        if (sortedBottomStructure.Count == 1)
        {
            bottomStructure = structureBottmChildren[0];
        }
        else
        {
            int maxY = sortedBottomStructure[0].TopLeftAreaCorner.y;
            sortedBottomStructure = sortedBottomStructure.Where(child => Mathf.Abs(maxY - child.TopLeftAreaCorner.y) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedBottomStructure.Count);
            bottomStructure = sortedBottomStructure[index];
        }

        var possibleNeighboursInTopStructure = structureAboveChildren.Where(
            child => GetValidXForNeighbourUpDown(
                bottomStructure.TopLeftAreaCorner,
                bottomStructure.TopRightAreaCorner,
                child.BottomLeftAreaCorner,
                child.BottomRightAreaCorner)
            != -1).OrderBy(child => child.BottomRightAreaCorner.y).ToList();
        if (possibleNeighboursInTopStructure.Count == 0)
        {
            topStructure = structure2;
        }
        else
        {
            topStructure = possibleNeighboursInTopStructure[0];
        }
        int x = GetValidXForNeighbourUpDown(
                bottomStructure.TopLeftAreaCorner,
                bottomStructure.TopRightAreaCorner,
                topStructure.BottomLeftAreaCorner,
                topStructure.BottomRightAreaCorner);
        while(x==-1 && sortedBottomStructure.Count > 1)
        {
            sortedBottomStructure = sortedBottomStructure.Where(child => child.TopLeftAreaCorner.x != topStructure.TopLeftAreaCorner.x).ToList();
            bottomStructure = sortedBottomStructure[0];
            x = GetValidXForNeighbourUpDown(
                bottomStructure.TopLeftAreaCorner,
                bottomStructure.TopRightAreaCorner,
                topStructure.BottomLeftAreaCorner,
                topStructure.BottomRightAreaCorner);
        }
        BottomLeftAreaCorner = new Vector2Int(x, bottomStructure.TopLeftAreaCorner.y);
        TopRightAreaCorner = new Vector2Int(x + this.corridorWidth, topStructure.BottomLeftAreaCorner.y);
    }

    private int GetValidXForNeighbourUpDown(Vector2Int bottomNodeLeft, 
        Vector2Int bottomNodeRight, Vector2Int topNodeLeft, Vector2Int topNodeRight)
    {
        if(topNodeLeft.x < bottomNodeLeft.x && bottomNodeRight.x < topNodeRight.x)
        {
            return YRouge_StructureHelper.CalculateMiddlePoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                bottomNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)
                ).x;
        }
        if(topNodeLeft.x >= bottomNodeLeft.x && bottomNodeRight.x >= topNodeRight.x)
        {
            return YRouge_StructureHelper.CalculateMiddlePoint(
                topNodeLeft+new Vector2Int(modifierDistanceFromWall,0),
                topNodeRight - new Vector2Int(this.corridorWidth+modifierDistanceFromWall,0)
                ).x;
        }
        if(bottomNodeLeft.x >= (topNodeLeft.x) && bottomNodeLeft.x <= topNodeRight.x)
        {
            return YRouge_StructureHelper.CalculateMiddlePoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceFromWall,0),
                topNodeRight - new Vector2Int(this.corridorWidth+modifierDistanceFromWall,0)

                ).x;
        }
        if(bottomNodeRight.x <= topNodeRight.x && bottomNodeRight.x >= topNodeLeft.x)
        {
            return YRouge_StructureHelper.CalculateMiddlePoint(
                topNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                bottomNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)

                ).x;
        }
        return -1;
    }
    private RelativePosition CheckPositionStructure1AgainstStructure2()
    {
        Vector2 middlePointStructure1 = new Vector2(
            (structure1.BottomLeftAreaCorner.x + structure1.TopRightAreaCorner.x) / 2,
            (structure1.BottomLeftAreaCorner.y + structure1.TopRightAreaCorner.y) / 2);
        Vector2 middlePointStructure2 = new Vector2(
            (structure2.BottomLeftAreaCorner.x + structure2.TopRightAreaCorner.x) / 2,
            (structure2.BottomLeftAreaCorner.y + structure2.TopRightAreaCorner.y) / 2);
        float angle = Mathf.Atan2(middlePointStructure2.y - middlePointStructure1.y,
            middlePointStructure2.x - middlePointStructure1.x)*Mathf.Rad2Deg;//计算两个结构之间的角度
        
        if((angle<45&&angle>=0)||(angle>-45&&angle<=0))
        {
            return RelativePosition.Right;
        }
        else if(angle>=45&&angle<135)
        {
            return RelativePosition.Up;
        }
        else if(angle>-135&&angle<-45)
        {
            return RelativePosition.Down;
        }
        else
        {
            return RelativePosition.Left;
        }
    }
}