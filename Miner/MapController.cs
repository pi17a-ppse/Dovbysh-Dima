using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Miner.Controllers
{
    /** 
 * Сапёр  
 *  
 * Класс используется для создание
 * игра сапер. 
 * 
 * @author Dima Dovbysh
 * @version 1.0. 0.
 * @copyright GNU Public License 
 * @todo Реализовать все методы 
 */
    public static class MapController
    {
        /** 
 * Размер картинки(количество иконок в картинке) 
 * 
 * Используетс для корректного 
 * поиска и использования
 * необходимых иконок 
 * 
 * @var int $mapSize 
 */

        public const int mapSize = 8;
        /** 
* Количество клеток на игровом поле
* 
* Используетс для отрисовки 
* необходимого количества 
* клеток и дальнейших расчетов
* 
* @var int $cellSize
*/
        public const int cellSize = 50;

        private static int currentPictureToSet = 0;

        public static int[,] map = new int[mapSize, mapSize];

        public static Button[,] buttons = new Button[mapSize, mapSize];

        public static Image spriteSet;

        private static bool isFirstStep;

        private static Point firstCoord;

        public static Form form;
        /** 
 * Работа c размером поля
 * 
 * Используется размер иконки, 
 * количество строк и столбцов, 
 * для определения размеров поля
 * 
 * @param $mapSize ширина 
 * @param $Height высота 
 */
        private static void ConfigureMapSize(Form current)
        {
            current.Width = mapSize * cellSize + 20;
            current.Height = (mapSize + 1) * cellSize;
        }
        private static void InitMap()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    map[i, j] = 0;
                }
            }
        }
        public static void Init(Form current)
        {
            form = current;
            currentPictureToSet = 0;
            isFirstStep = true;
            spriteSet = new Bitmap(Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName.ToString(), "tiles.png"));
            ConfigureMapSize(current);
            InitMap();
            InitButtons(current);
        }
        /** 
* Работа c отрисовкой поля
* 
* Используется для генерации поля, 
* определенным алгоритмом
* 
* @param $mapSize размер иконки
* @param $cellSize количество полей
*/
        private static void InitButtons(Form current)
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Button button = new Button();
                    button.Location = new Point(j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.Image = FindNeededImage(0, 0);
                    button.MouseUp += new MouseEventHandler(OnButtonPressedMouse);
                    current.Controls.Add(button);
                    buttons[i, j] = button;
                }
            }
        }
        /** 
* Чнение нажатой кнопки на мыше
* 
* Используется для того, 
* что бы понимать, какую 
* кнопку пользователь нажал: лкм или пкм
* 
*/
        private static void OnButtonPressedMouse(object sender, MouseEventArgs e)//Чтение, какая кнопка мыши была нажата
        {
            Button pressedButton = sender as Button;
            switch (e.Button.ToString())
            {
                case "Right":
                    OnRightButtonPressed(pressedButton);
                    break;
                case "Left":
                    OnLeftButtonPressed(pressedButton);
                    break;
            }
        }
        /** 
* Собитые, при нажатии на пкм
* 
* Используется для определения 
* позиции мышки, при нажатии пкм и 
* загрузке иконки флажка на 
* определенную позицию
* 
* @param $posX координаты Х
* @param $posY координаты Y
*/
        private static void OnRightButtonPressed(Button pressedButton)//Событие при нажатии пкм
        {
            currentPictureToSet++;
            currentPictureToSet %= 3;
            int posX = 0;
            int posY = 0;
            switch (currentPictureToSet)
            {
                case 0:
                    posX = 0;
                    posY = 0;
                    break;
                case 1:
                    posX = 0;
                    posY = 2;
                    break;
                case 2:
                    posX = 2;
                    posY = 2;
                    break;
            }
            pressedButton.Image = FindNeededImage(posX, posY);//Событие при нажатии лкм
        }
        /** 
* Собитые, при нажатии на лкм
* 
* Используется для определения 
* позиции мышки, пр нажатии лкм и 
* работы алгоритмов игры
* 
* @param $iButton координаты Х
* @param $jButton координаты Y
*/
        private static void OnLeftButtonPressed(Button pressedButton)
        {
            pressedButton.Enabled = false;
            int iButton = pressedButton.Location.Y / cellSize;
            int jButton = pressedButton.Location.X / cellSize;
            if (isFirstStep)
            {
                firstCoord = new Point(jButton, iButton);
                SeedMap();
                CountCellBomb();
                isFirstStep = false;
            }
            OpenCells(iButton, jButton);

            if (map[iButton, jButton] == -1)
            {
                ShowAllBombs(iButton, jButton);
                MessageBox.Show("Поражение!");
                form.Controls.Clear();
                Init(form);
            }
        }
        /** 
* Показ всех бомб
* 
* Используется для показа 
* нахождения все бомб на карте 
* при поражении
* 
*/
        private static void ShowAllBombs(int iBomb, int jBomb)
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (i == iBomb && j == jBomb)
                        continue;
                    if (map[i, j] == -1)
                    {
                        buttons[i, j].Image = FindNeededImage(3, 2);
                    }
                }
            }
        }
        /** 
* Поиск необходимой иконки
* 
* Используется для поиска 
* необходимых иконок, таких
* как: цифры, флажек, бомба и тд. 
* А также для отрисовки этих иконок
* 
* @param $iButton координаты Х
* @param $jButton координаты Y
* 
* @return image Возвращает текущую найденную иконку 
*/
        public static Image FindNeededImage(int xPos, int yPos)
        {
            Image image = new Bitmap(cellSize, cellSize);
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(spriteSet, new Rectangle(new Point(0, 0), new Size(cellSize, cellSize)), 0 + 32 * xPos, 0 + 32 * yPos, 33, 33, GraphicsUnit.Pixel);


            return image;
        }
        private static void SeedMap()
        {
            Random r = new Random();
            int number = r.Next(7, 15);

            for (int i = 0; i < number; i++)
            {
                int posI = r.Next(0, mapSize - 1);
                int posJ = r.Next(0, mapSize - 1);

                while (map[posI, posJ] == -1 || (Math.Abs(posI - firstCoord.Y) <= 1 && Math.Abs(posJ - firstCoord.X) <= 1))
                {
                    posI = r.Next(0, mapSize - 1);
                    posJ = r.Next(0, mapSize - 1);
                }
                map[posI, posJ] = -1;
            }
        }
        private static void CountCellBomb()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[i, j] == -1)
                    {
                        for (int k = i - 1; k < i + 2; k++)
                        {
                            for (int l = j - 1; l < j + 2; l++)
                            {
                                if (!IsInBorder(k, l) || map[k, l] == -1)
                                    continue;
                                map[k, l] = map[k, l] + 1;
                            }
                        }
                    }
                }
            }
        }
        /** 
* Открытие полей
* 
* Используется для открытия
* поля и загруски необходимой 
* иконки
* 
*/
        private static void OpenCell(int i, int j)
        {
            buttons[i, j].Enabled = false;

            switch (map[i, j])
            {
                case 1:
                    buttons[i, j].Image = FindNeededImage(1, 0);
                    break;
                case 2:
                    buttons[i, j].Image = FindNeededImage(2, 0);
                    break;
                case 3:
                    buttons[i, j].Image = FindNeededImage(3, 0);
                    break;
                case 4:
                    buttons[i, j].Image = FindNeededImage(4, 0);
                    break;
                case 5:
                    buttons[i, j].Image = FindNeededImage(0, 1);
                    break;
                case 6:
                    buttons[i, j].Image = FindNeededImage(1, 1);
                    break;
                case 7:
                    buttons[i, j].Image = FindNeededImage(2, 1);
                    break;
                case 8:
                    buttons[i, j].Image = FindNeededImage(3, 1);
                    break;
                case -1:
                    buttons[i, j].Image = FindNeededImage(1, 2);
                    break;
                case 0:
                    buttons[i, j].Image = FindNeededImage(0, 0);
                    break;
            }
        }
        private static void OpenCells(int i, int j)
        {
            OpenCell(i, j);

            if (map[i, j] > 0)
                return;

            for (int k = i - 1; k < i + 2; k++)
            {
                for (int l = j - 1; l < j + 2; l++)
                {
                    if (!IsInBorder(k, l))
                        continue;
                    if (!buttons[k, l].Enabled)
                        continue;
                    if (map[k, l] == 0)
                        OpenCells(k, l);
                    else if (map[k, l] > 0)
                        OpenCell(k, l);
                }
            }
        }
        /** 
* Проверка на выход за границы
* 
* Используется проверки, не выходит ли 
* поле за границы формы при отрисовке
* 
* @return bool Возвращает либо 1 либо 0 
*/
        private static bool IsInBorder(int i, int j)
        {
            if (i < 0 || j < 0 || j > mapSize - 1 || i > mapSize - 1)
            {
                return false;
            }
            return true;
        }
    }
}
