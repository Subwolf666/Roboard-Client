using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roboard
{
    public static class StaticUtilities
    {
        public const int numberOfServos = 24;
        public static int numberOfMotions = 0;
        public static int numberOfDataTableItems = 0;
        public const string messageSeperator = ",";

        public const double conversionAngle = 2.9; // rcb servo value times 2,9
        public const int conversionSpeed = 15; // playtime = 15 times rcb speed

        public const int numberOfAcceleroMeterAxis = 3;
        public const int numberOfMagnetoMeterAxis = 3;
        public const int numberOfGyroscopeAxis = 8;

        public const string SectionGraphicalEdit = "GraphicalEdit";
        public const string SectionItem = "Item";
        public const string SectionLink = "Link";

        public const string GraphicalEditType = "Type";
        public const string GraphicalEditWidth = "Width";
        public const string GraphicalEditHeight = "Height";
        public const string GraphicalEditItems = "Items";
        public const string GraphicalEditLinks = "Links";
        public const string GraphicalEditStart = "Start";
        public const string GraphicalEditName = "Name";
        public const string GraphicalEditCtrl = "Ctrl";

        public readonly static string[] GraphicalEdit = new string[]
        {
            GraphicalEditType,
            GraphicalEditWidth,
            GraphicalEditHeight,
            GraphicalEditItems,
            GraphicalEditLinks,
            GraphicalEditStart,
            GraphicalEditName,
            GraphicalEditCtrl
        };

        public const string ItemName = "Name";
        public const string ItemWidth = "Width";
        public const string ItemHeight = "Height";
        public const string ItemLeft = "Left";
        public const string ItemTop = "Top";
        public const string ItemColor = "Color";
        public const string ItemType = "Type";
        public const string ItemPrm = "Prm";

        public readonly static string[] Item = new string[]
        {
            ItemName,
            ItemWidth,
            ItemHeight,
            ItemLeft,
            ItemTop,
            ItemColor,
            ItemType,
            ItemPrm
        };

        public const string LinkMain = "Main";
        public const string LinkOrigin = "Origin";
        public const string LinkFinal = "Final";
        public const string LinkPoint = "Point";

        public readonly static string[] Link = new string[]
        {
            LinkMain,
            LinkOrigin,
            LinkFinal,
            LinkPoint
        };

    }
}
