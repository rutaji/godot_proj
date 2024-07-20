using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dream.scrips
{
    public partial class  TileManager : Node2D
    {
        private static TileMap TileMap;

        public override void _Ready()
        {
            base._Ready();
            TileMap = GetNode<TileMap>("T");
        }
        public static Vector2I getPosition(Rid rid)
        {
            return TileMap.GetCoordsForBodyRid(rid);
        }
        public static Vector2 getGlobal(Vector2I pos)
        {
            return TileMap.MapToLocal(pos);
        }
        public static int GetTileValue(Vector2I TileMapCoords) 
        {
            
            TileData data = TileMap.GetCellTileData(0, TileMapCoords);
            if (data != null)
            {
                return data.GetCustomData("id").AsInt32();
            }
            return 0;
        }

        internal static void RemoveSpawn(Vector2 position)
        {
            Vector2I pos = TileMap.LocalToMap(position);
            TileData data = TileMap.GetCellTileData(0, pos);
            if  (data != null && data.GetCustomData("id").AsInt32() == 3) 
            {
                TileMap.SetCell(0, pos,0,new Vector2I(2,0),0);
            }
        }

        internal static void SetSpawn(Vector2I pos)
        {
            TileMap.SetCell(0, pos, 0, new Vector2I(3, 0), 0);
        }

        

        internal static Vector2I getPosition(Vector2 position)
        {
            return TileMap.LocalToMap(position);
        }
    }
}
