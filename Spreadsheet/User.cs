﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SS
{
    public class User
    {
        private int ID;
        private string name;
        private int col;
        private int row;
        private Color color;
        List<Color> colors = new List<Color>() {Color.Blue, Color.Red, Color.Purple, Color.Green, Color.Orange, Color.Brown, Color.Lime, Color.Fuchsia, Color.Aqua, Color.Yellow};


        public User(int ID, string name, int col, int row, Color color)
        {
            this.ID = ID;
            this.name = name;
            this.col = col;
            this.row = row;
            this.color = color;
        }

        public int getID()
        {
            return ID;
        }

        public string getName()
        {
            return name;
        }

        public int getCol()
        {
            return this.col;
        }

        public int getRow()
        {
            return this.row;
        }

        public Color getColor()
        {
            return this.color;
        }

        public void setRow(int row)
        {
            this.row = row;
        }

        public void setCol(int col)
        {
            this.col = col;
        }

        public void setColor(Color color)
        {
            this.color = color;
        }
    }
}
