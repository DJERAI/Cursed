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
    public partial class UC_Cars : UserControl
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
        //Переменная для ID записи в БД, выбранной в гриде. Пока она не содердит значения, лучше его инициализировать с 0
        //что бы в БД не отправлялся null
        string id_selected_rows = "0";
        string index_rows5;
        string idCar;
        string idMarka;
        string idModel;
        string NumberTS;
        string id_rows5;
        
        public UC_Cars()
        {
            InitializeComponent();
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
        public bool DeleteCar(string idCar,string idMarka,string idModel, string NumberTS)
        {
            //определяем переменную, хранящую количество вставленных строк
            int InsertCount = 0;
            //Объявляем переменную храняющую результат операции
            bool result = false;
            // открываем соединение
            conn.Open();
            // запрос удаления данных
            string query = $"DELETE idCar FROM t_Cars WHERE idCar='{idCar}'";
            string query2 = $"DELETE idMarka FROM t_Marka WHERE (idMarka='{idMarka}')";
            string query3 = $"DELETE idModel FROM t_Model WHERE (idModel='{idModel}')";
            string query4 = $"DELETE NumberTS FROM t_Cars WHERE (NumberTS='{NumberTS}')";
            try
            { 
            MySqlCommand command = new MySqlCommand(query, conn);
            MySqlCommand command1 = new MySqlCommand(query2, conn);
            MySqlCommand command2 = new MySqlCommand(query3, conn);
                MySqlCommand command3 = new MySqlCommand(query4, conn);
                // выполняем запрос
                InsertCount = command.ExecuteNonQuery();
            InsertCount = command1.ExecuteNonQuery();
            InsertCount = command2.ExecuteNonQuery();
                InsertCount = command3.ExecuteNonQuery();   
            }
            catch
            {
                InsertCount = 0;
            }
            finally
            {
                conn.Close();
                if (InsertCount != 0)
                { 
                result = true;
                reload_list();
                }
            }
            return result;

        }
        public void GetListCar()
        {
            //Запрос для вывода строк в БД
            string commandStr = $"SELECT t_Cars.NumberTS AS 'Номер автомобиля', t_Marka.titleMarks AS 'Марка автомобиля', t_Model.titleModel AS 'Модель автомобиля' FROM (t_Marka INNER JOIN t_Model ON t_Marka.idMarka = t_Model.idMarka) INNER JOIN t_Cars ON t_Model.idModel = t_Cars.idModel;"; 
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

        private void UC_Cars_Load(object sender, EventArgs e)
        {
            string connStr = "server=caseum.ru;port=33333;user=st_1_22_19;database=st_1_22_19;password=97035229;";
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

            GetComboBox1();
            GetComboBox3();

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            string index_selected_rows;
            //Индекс выбранной строки
            index_selected_rows = dataGridView1.SelectedCells[0].RowIndex.ToString();
            //ID конкретной записи в Базе данных, на основании индекса строки
            id_selected_rows = dataGridView1.Rows[Convert.ToInt32(index_selected_rows)].Cells[0].Value.ToString();
            dataGridView1.Rows.RemoveAt(Convert.ToInt32(index_rows5));
            DeleteCar(idCar,idMarka,idModel,NumberTS);
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!e.RowIndex.Equals(-1) && !e.ColumnIndex.Equals(-1) && e.Button.Equals(MouseButtons.Left))
            {
                dataGridView1.CurrentCell = dataGridView1[e.ColumnIndex, e.RowIndex];

                dataGridView1.CurrentRow.Selected = true;

                index_rows5 = dataGridView1.SelectedCells[0].RowIndex.ToString();
                                
                idCar = dataGridView1.Rows[Convert.ToInt32(index_rows5)].Cells[0].Value.ToString();

                idMarka = dataGridView1.Rows[Convert.ToInt32(index_rows5)].Cells[1].Value.ToString();

                idModel = dataGridView1.Rows[Convert.ToInt32(index_rows5)].Cells[2].Value.ToString();

                NumberTS = dataGridView1.Rows[Convert.ToInt32(index_rows5)].Cells[3].Value.ToString();

               GetSelectedIDString();

            }
        }
        public bool InsertCars(string Marka, string Model, string Number, string Col)
        {
            //определяем переменную, хранящую количество вставленных строк
            int InsertCount = 0;
            //Объявляем переменную храняющую результат операции
            bool result = false;
            // открываем соединение
            conn.Open();
            // запросы
            // запрос вставки данных
            string query = $"INSERT INTO t_Marka (titleMarks) VALUES ('{Marka}')";
            string query1 = $"INSERT INTO t_Model (titleModel) VALUES ('{Model}')";
            string query2 = $"INSERT INTO t_Cars (NumberTS) VALUES ('{Number}')";
            string query3 = $"INSERT INTO t_Color (Color) VALUES ('{Col}')";
            
            try
            {
                // объект для выполнения SQL-запроса
                MySqlCommand command = new MySqlCommand(query, conn);
                MySqlCommand command1 = new MySqlCommand(query1, conn);
                MySqlCommand command2 = new MySqlCommand(query2, conn);
                MySqlCommand command3 = new MySqlCommand(query3, conn);

                // выполняем запрос
                InsertCount = command.ExecuteNonQuery();
                InsertCount = command1.ExecuteNonQuery();
                InsertCount = command2.ExecuteNonQuery();
                InsertCount = command3.ExecuteNonQuery();

                // закрываем подключение к БД
            }
            catch
            {
                //Если возникла ошибка, то запрос не вставит ни одной строки
                InsertCount = 0;
            }
            finally
            {
                //Но в любом случае, нужно закрыть соединение
                conn.Close();
                //Ессли количество вставленных строк было не 0, то есть вставлена хотя бы 1 строка
                if (InsertCount != 0)
                {
                    //то результат операции - истина
                    result = true;
                    
                }
            }
            reload_list();
            //Вернём результат операции, где его обработает алгоритм
            return result;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string Marka = toolStripTextBox1.Text;
            string Model = toolStripTextBox2.Text;
            string Number = toolStripTextBox3.Text;
            string Col = toolStripTextBox4.Text;
            
            //Если метод вставки записи в БД вернёт истину, то просто обновим список и увидим вставленное значение
            InsertCars( Marka,  Model,  Number, Col);
            reload_list();
            
            //Иначе произошла какая то ошибка и покажем пользователю уведомление
                       
        }
        public void GetComboBox1()
        {
            //Формирование списка статусов
            DataTable list_marka_table = new DataTable();
            MySqlCommand list_marka_command = new MySqlCommand();
            //Открываем соединение
            conn.Open();
            //Формируем столбцы для комбобокса списка ЦП
            list_marka_table.Columns.Add(new DataColumn("idMarka", System.Type.GetType("System.Int32")));
            list_marka_table.Columns.Add(new DataColumn("titleMarks", System.Type.GetType("System.String")));
            //Настройка видимости полей комбобокса
            comboBox1.DataSource = list_marka_table;
            comboBox1.DisplayMember = "titleMarks";
            comboBox1.ValueMember = "idMarka";
            //Формируем строку запроса на отображение списка статусов прав пользователя
            string sql_list_model = "SELECT idMarka, titleMarks FROM t_Marka;SELECT NumberTS FROM t_Cars";
            list_marka_command.CommandText = sql_list_model;
            list_marka_command.Connection = conn;
            //Формирование списка ЦП для combobox'a
            MySqlDataReader list_model_reader;
            try
            {
                //Инициализируем ридер
                list_model_reader = list_marka_command.ExecuteReader();
                while (list_model_reader.Read())
                {
                    DataRow rowToAdd = list_marka_table.NewRow();
                    rowToAdd["idMarka"] = Convert.ToInt32(list_model_reader[0]);
                    rowToAdd["titleMarks"] = list_model_reader[1].ToString();
                    list_marka_table.Rows.Add(rowToAdd);
                }
                list_model_reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка чтения списка ЦП \n\n" + ex, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            finally
            {
                conn.Close();
            }
        }
        public void GetComboBox2(string idMarka)
        {
            //Формирование списка статусов
            DataTable list_model_table = new DataTable();
            MySqlCommand list_model_command = new MySqlCommand();
            //Открываем соединение
            conn.Open();
            //Формируем столбцы для комбобокса списка ЦП
            list_model_table.Columns.Add(new DataColumn("idModel", System.Type.GetType("System.Int32")));
            list_model_table.Columns.Add(new DataColumn("titleModel", System.Type.GetType("System.String")));
            //Настройка видимости полей комбобокса
            comboBox2.DataSource = list_model_table;
            comboBox2.DisplayMember = "titleModel";
            comboBox2.ValueMember = "idModel";
            //Формируем строку запроса на отображение списка статусов прав пользователя
            string sql_list_users = $"SELECT idModel, titleModel FROM t_Model WHERE idMarka = {idMarka}";
            list_model_command.CommandText = sql_list_users;
            list_model_command.Connection = conn;
            //Формирование списка ЦП для combobox'a
            MySqlDataReader list_model_reader;
            try
            {
                //Инициализируем ридер
                list_model_reader = list_model_command.ExecuteReader();
                while (list_model_reader.Read())
                {
                    DataRow rowToAdd = list_model_table.NewRow();
                    rowToAdd["idModel"] = Convert.ToInt32(list_model_reader[0]);
                    rowToAdd["titleModel"] = list_model_reader[1].ToString();
                    list_model_table.Rows.Add(rowToAdd);
                }
                list_model_reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка чтения списка ЦП \n\n" + ex, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            finally
            {
                conn.Close();
            }
            reload_list();
        }
        public void GetComboBox3()
        {
            //Формирование списка статусов
            DataTable list_marka_table = new DataTable();
            MySqlCommand list_marka_command = new MySqlCommand();
            //Открываем соединение
            conn.Open();
            //Формируем столбцы для комбобокса списка ЦП
            list_marka_table.Columns.Add(new DataColumn("idColor", System.Type.GetType("System.Int32")));
            list_marka_table.Columns.Add(new DataColumn("Color", System.Type.GetType("System.String")));
            //Настройка видимости полей комбобокса
            comboBox3.DataSource = list_marka_table;
            comboBox3.DisplayMember = "Color";
            comboBox3.ValueMember = "idColor";
            //Формируем строку запроса на отображение списка статусов прав пользователя
            string sql_list_model = "SELECT idColor, Color FROM t_Color";
            list_marka_command.CommandText = sql_list_model;
            list_marka_command.Connection = conn;
            //Формирование списка ЦП для combobox'a
            MySqlDataReader list_model_reader;
            try
            {
                //Инициализируем ридер
                list_model_reader = list_marka_command.ExecuteReader();
                while (list_model_reader.Read())
                {
                    DataRow rowToAdd = list_marka_table.NewRow();
                    rowToAdd["idColor"] = Convert.ToInt32(list_model_reader[0]);
                    rowToAdd["Color"] = list_model_reader[1].ToString();
                    list_marka_table.Rows.Add(rowToAdd);
                }
                list_model_reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка чтения списка ЦП \n\n" + ex, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            finally
            {
                conn.Close();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Enabled = true;
            //Заполнение Combobox2 теми подкатегориями, которые относятся к выбранной категории
            GetComboBox2(comboBox1.SelectedValue.ToString());
            //Установка пустой строки по умолчанию в ComboBox2
            comboBox2.Text = "";

            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Чистим виртуальную таблицу
            table.Clear();
            //Вызываем метод наполнения ДатаГрид только теми объектами, которые подходят по условию
            GetListCar();
            //Видимость полей в гриде
            dataGridView1.Columns[0].Visible = true;
            dataGridView1.Columns[1].Visible = true;
            dataGridView1.Columns[2].Visible = true;
            
            //Ширина полей
            dataGridView1.Columns[0].FillWeight = 10;
            dataGridView1.Columns[1].FillWeight = 70;
            dataGridView1.Columns[2].FillWeight = 20;
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
        }
    }
}
