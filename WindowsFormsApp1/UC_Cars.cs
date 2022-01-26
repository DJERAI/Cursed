using System;
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
            string query = $"DELETE * FROM t_Cars WHERE idCar='{idCar}'";
            string query2 = $"DELETE FROM t_Marka WHERE (idMarka='{idMarka}')";
            string query3 = $"DELETE FROM t_Model WHERE (idModel='{idModel}')";
            string query4 = $"DELETE FROM t_Cars WHERE (NumberTS='{NumberTS}')";
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
            string commandStr = $"SELECT t_Cars.idCar, t_Marka.titleMarks, t_Model.titleModel, t_Cars.NumberTS FROM(t_Marka INNER JOIN t_Model ON t_Marka.idMarka = t_Model.idMarka) INNER JOIN t_Cars ON t_Model.idModel = t_Cars.idModel";
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
            dataGridView1.Columns[3].Visible = true;
            

            //Ширина полей
            dataGridView1.Columns[0].FillWeight = 15;
            dataGridView1.Columns[1].FillWeight = 40;
            dataGridView1.Columns[2].FillWeight = 15;
            dataGridView1.Columns[3].FillWeight = 15;
           
            //Режим для полей "Только для чтения"
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.Columns[3].ReadOnly = true;
            
            //Растягивание полей грида
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            
            //Убираем заголовки строк
            dataGridView1.RowHeadersVisible = false;
            //Показываем заголовки столбцов
            dataGridView1.ColumnHeadersVisible = false;
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

                //MessageBox.Show($"{idCar},{idMarka},{idModel},{NumberTS}");

            }
        }
        public bool InsertCars(string Marka, string Model, string Number)
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
            try
            {
                // объект для выполнения SQL-запроса
                MySqlCommand command = new MySqlCommand(query, conn);
                MySqlCommand command1 = new MySqlCommand(query1, conn);
                MySqlCommand command2 = new MySqlCommand(query2, conn);
                // выполняем запрос
                InsertCount = command.ExecuteNonQuery();
                InsertCount = command1.ExecuteNonQuery();
                InsertCount = command2.ExecuteNonQuery();
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
            //Если метод вставки записи в БД вернёт истину, то просто обновим список и увидим вставленное значение
            if (InsertCars(Marka, Model, Number))
            {
                GetListCar();
            }
            //Иначе произошла какая то ошибка и покажем пользователю уведомление
            else
            {
                MessageBox.Show("Произошла ошибка.", "Ошибка");
            }
        }
    }
}
