﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    public partial class UC_Drivers : UserControl
    {
        MySqlConnection conn;
        //DataAdapter представляет собой объект Command , получающий данные из источника данных.
        private MySqlDataAdapter MyDA = new MySqlDataAdapter();
        //Объявление BindingSource, основная его задача, это обеспечить унифицированный доступ к источнику данных.
        private BindingSource bSource = new BindingSource();
        //DataSet - расположенное в оперативной памяти представление данных, обеспечивающее согласованную реляционную программную 
        //модель независимо от источника данных.DataSet представляет полный набор данных, включая таблицы, содержащие, упорядочивающие 
        //и ограничивающие данные, а также связи между таблицами.
        private DataSet ds = new DataSet();
        //Представляет одну таблицу данных в памяти.
        private DataTable table = new DataTable();
        // строка подключения к БД
        string connStr = "server=caseum.ru;port=33333;user=st_1_22_19;database=st_1_22_19;password=97035229;";
        string id_selected_rows = "0";

        public UC_Drivers()
        {
            InitializeComponent();
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void UC_Drivers_Load(object sender, EventArgs e)
        {
            conn = new MySqlConnection(connStr);
            GetListCar();
            //Видимость полей в гриде
            dataGridView1.Columns[0].Visible = true;
            dataGridView1.Columns[1].Visible = true;
            dataGridView1.Columns[2].Visible = true;


            //Ширина полей
            dataGridView1.Columns[0].FillWeight = 150;
            dataGridView1.Columns[1].FillWeight = 150;
            dataGridView1.Columns[2].FillWeight = 150;

            //Режим для полей "Только для чтения"
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = true;

            //Растягивание полей грида
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            //Убираем заголовки строк
            dataGridView1.RowHeadersVisible = true;
            //Показываем заголовки столбцов
            dataGridView1.ColumnHeadersVisible = true;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Переменная для хранения даты
            string dateDriverFromDB;
            //Объявлем переменную для запроса в БД
            string selected_id_driver = textBox3.Text;
            // устанавливаем соединение с БД
            conn.Open();
            // запрос
            string sql = $"SELECT * FROM t_Driver WHERE idDriver={selected_id_driver}";
            // объект для выполнения SQL-запроса
            MySqlCommand command = new MySqlCommand(sql, conn);
            // объект для чтения ответа сервера
            MySqlDataReader reader = command.ExecuteReader();
            // читаем результат
            while (reader.Read())
            {
                // элементы массива [] - это значения столбцов из запроса SELECT
                textBox4.Text = reader[1].ToString();
                dateDriverFromDB = reader[2].ToString();
                //Переводим дату из БД в дату для dateTimePicker и присваиваем
                dateTimePicker2.Value = Convert.ToDateTime(dateDriverFromDB);
            }
            reader.Close(); // закрываем reader
            // закрываем соединение с БД
            conn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                //Открываем соединение
                conn.Open();

                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO t_Driver (fioDriver,dateOfbirthDriver,phoneDriver) " +
                   "VALUES (@name, @date)", conn))
                {
                    //Использование параметров в запросах. Это повышает безопасность работы программы
                    cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = textBox1.Text;
                    cmd.Parameters.Add("@date", MySqlDbType.Timestamp).Value = dateTimePicker2.Value;
                    cmd.Parameters.Add("@number", MySqlDbType.VarChar).Value = textBox2.Text;

                    int insertedRows = cmd.ExecuteNonQuery();
                    // закрываем подключение  БД
                    conn.Close();

                }
            }
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!e.RowIndex.Equals(-1) && !e.ColumnIndex.Equals(-1) && e.Button.Equals(MouseButtons.Right))
            {
                dataGridView1.CurrentCell = dataGridView1[e.ColumnIndex, e.RowIndex];
                //dataGridView1.CurrentRow.Selected = true;
                dataGridView1.CurrentCell.Selected = true;
                //Метод получения ID выделенной строки в глобальную переменную
                GetSelectedIDString();
            }
        }
        public void GetSelectedIDString()
        {
            //Переменная для индекс выбранной строки в гриде
            string index_selected_rows;
            //Индекс выбранной строки
            index_selected_rows = dataGridView1.SelectedCells[0].RowIndex.ToString();
            //ID конкретной записи в Базе данных, на основании индекса строки
            id_selected_rows = dataGridView1.Rows[Convert.ToInt32(index_selected_rows)].Cells[0].Value.ToString();
        }
        public void reload_list()
        {
            //Чистим виртуальную таблицу
            table.Clear();
            //Вызываем метод получения записей, который вновь заполнит таблицу
            GetListCar();
        }
        public void GetListCar()
        {
            //Запрос для вывода строк в БД
            string commandStr = $"SELECT t_Driver.fioDriver, t_Driver.phoneDriver, t_Driver.dateOfbirthDriver, t_Driver.idCar FROM t_Driver INNER JOIN t_Cars ON t_Driver.idCar = t_Cars.idCar;";
            ;
            //Открываем соединение
            conn.Open();
            //Объявляем команду, которая выполнить запрос в соединении conn
            MyDA.SelectCommand = new MySqlCommand(commandStr, conn);
            //Заполняем таблицу записями из БД
            MyDA.Fill(table);
            //Указываем, что источником данных в bindingsource является заполненная выше таблица
            bSource.DataSource = table;
            //Указываем, что источником данных ДатаГрида является bindingsource 
            dataGridView1.DataSource = bSource;
            //Закрываем соединение
            conn.Close();


        }
    }
}
