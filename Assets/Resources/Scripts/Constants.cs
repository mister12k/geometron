using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants{

    public static float TILE_WIDTH = 0.2f;
	public static float TILE_GAP = 2.01f;
	public static float UNIT_TILE_DIFF= 0.5f;	//	Height between tiles and centre of shapes

	public static Color32 COLOR_SHAPE_SELECTED = new Color32(0x99, 0x66, 0x99, 0xFF);
	public static Color32 COLOR_SHAPE_OVER = new Color32 (0xC1, 0xF6, 0xFC, 0xFF);
	public static Color32 COLOR_MOVE_AREA = new Color32(0x00, 0x00, 0x80, 0xFF);
	public static Color32 COLOR_MOVE_OVER = new Color32 (0xFF, 0x69, 0xB4, 0xFF);
    public static Color32 COLOR_TILE_NORMAL = Color.white;
    public static Color32 COLOR_TILE_OVER = new Color32(0xBB, 0xBB, 0xBB, 0xFF);
    public static Color32 COLOR_TILE_GOAL = new Color32(0xF8, 0x23, 0x23, 0xFF);
    public static Color32 COLOR_TILE_GOAL_ACTIVE = new Color32(0x00, 0x64, 0x00, 0xFF);
    public static Color32 COLOR_TILE_PRESSURE = new Color32(0xFF,0xE3,0x00,0xFF);
    public static Color32 COLOR_INTERACT_AREA = new Color32(0x66,0xE5,0x47,0xFF);
    public static Color32 COLOR_INTERACT_OVER = new Color32(0xE5, 0x47, 0x66, 0xFF);
    public static Color32 COLOR_BUTTON_CLICKED = new Color32(0xC8, 0xC8, 0xC8, 0xFF);
    public static Color32 COLOR_BUTTON_UNCLICKED = Color.white;


    public static int TURNS_LEVEL1 = 3;
    public static int TURNS_LEVEL2 = 5;
    public static int TURNS_LEVEL3 = 6;


}
