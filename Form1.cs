using System;
using System.Windows.Forms;
using Pathfinding.Dstar;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            
            //Movement = new SerialPortManager();
            InitializeComponent();
        }


        private void ButtonStart_Click(object sender, EventArgs e)
        {

            Pather Pathfinder = new Pather();
            Robot Rover = new Robot(0, 3);
            string text = "";
            Pathfinder.GenerateMap(0, 3);
            richPathCost.Text += "Map Done" + Environment.NewLine;
            Pathfinder.GenerateCostMap(Rover, 250, 250);
            richPathCost.Text += "Cost Map Done" + Environment.NewLine;
            Pathfinder.FindClosedPath();
            richPathCost.Text += "Pathing Done" + Environment.NewLine;
            foreach (var tile in Pathfinder.ShortestPath)
            {
                text += $"{tile.X }, {tile.Y}\r\n";
            }
            text += "Blocked coords";
            foreach (var tile in Pathfinder.BlockedList)
            {
                    text += $"{tile.X }, {tile.Y}\r\n";
            }
            richPathCost.Text += text;
        }

        public void UpdateCost(string text)
        {
            this.BeginInvoke((Action)(() =>
            {
                richPathCost.Text += text + Environment.NewLine;
            }));
        }

    }
}