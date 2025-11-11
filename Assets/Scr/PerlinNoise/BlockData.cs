using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//ºí·Ï Å¸ÀÔ Á¤ÀÇ
public enum BlockType
{
    Air,                    //°ø±â
    Grass,                  //ÀÜµð
    Dirt,                   //Èë
    Stone,                  //µ¹
    Bedrock,                //±â¹Ý¾Ï
    Wood,                   //³ª¹«
    Leaf,                   //³ª¹µÀÙ
    Water,                  //¹°
    Sand,                   //¸ð·¡
    CoalOre,                //¼®Åº ±¤¸Æ
    IronOre,                //Ã¶ ±¤¸Æ
    GoldOre,                //±Ý ±¤¸Æ
    DiamondOre,             //´ÙÀÌ¾Æ ±¤¸Æ
}

[System.Serializable]
public class BlockData
{
    public BlockType blockType;
    public Color blockColor;
    public bool isSolid;

    public BlockData(BlockType type)
    {
        blockType = type;
        isSolid = type != BlockType.Air;

        //ºí·Ï »ö»ó ¼³Á¤
        switch (type)
        {
            case BlockType.Grass:
                blockColor = new Color(0.2f, 0.8f, 0.2f);
                break;
            case BlockType.Dirt:
                blockColor = new Color(0.6f, 0.4f, 0.2f);
                break;
            case BlockType.Stone:
                blockColor = new Color(0.5f, 0.5f, 0.5f);
                break;
            case BlockType.Bedrock:
                blockColor = new Color(0.2f, 0.2f, 0.2f);
                break;
            case BlockType.Wood:
                blockColor = new Color(0.6f, 0.3f, 0.1f);
                break;
            case BlockType.Leaf:
                blockColor = new Color(0.1f, 0.6f, 0.1f);
                break;
            case BlockType.Water:
                blockColor = new Color(0.2f, 0.4f, 0.9f);
                isSolid = false;    //¹°Àº Åë°ú °¡´É
                break;
            case BlockType.Sand:
                blockColor = new Color(0.9f, 0.85f, 0.6f);
                break;
            case BlockType.CoalOre:
                blockColor = new Color(0.3f, 0.3f, 0.3f);
                break;
            case BlockType.IronOre:
                blockColor = new Color(0.7f, 0.6f, 0.5f);
                break;
            case BlockType.GoldOre:
                blockColor = new Color(0.9f, 0.8f, 0.2f);
                break;
            case BlockType.DiamondOre:
                blockColor = new Color(0.3f, 0.8f, 0.9f);
                break;

            default:
                blockColor = Color.clear;
                isSolid = false;
                break;
        }
    }
}