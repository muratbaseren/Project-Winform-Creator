using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyCodeLibrary.Serialization;
using System.IO;

namespace ProjectCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private List<Field> FieldList = new List<Field>();
        private string JsonFilePath = Application.StartupPath + "//fields.json";
        private string JsonDataFilePath = Application.StartupPath + "//fields_data.json";
        private List<Dictionary<string, string>> Hepsi = new List<Dictionary<string, string>>();

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Field f = new Field();
            f.FieldName = txtFName.Text;
            f.FieldType = cmbFType.Text;

            // Önce listem'e ekliyorum.
            FieldList.Add(f);

            // Sonra listbox dolduruyorum.
            ListFields();
        }

        private void ListFields()
        {
            lstFields.DataSource = null;
            lstFields.DataSource = FieldList;
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (lstFields.SelectedIndex > -1)
            {
                // Önce listem'den siliyorum.
                FieldList.RemoveAt(lstFields.SelectedIndex);

                // Sonra listbox dolduruyorum.
                ListFields();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Json js = new Json();
            js.Serialize(JsonFilePath, FieldList);

            MessageBox.Show("Kaydedildi");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Eğer JsonFilePath dosyası varsa..
            if (File.Exists(JsonFilePath))
            {
                Json js = new Json();
                object obj = js.Deserialize(JsonFilePath, FieldList.GetType());

                // Deserialize işlemi sonrası dönen obje(object) tip dönüşümü yapılır.
                // FieldList listemize eşitlenir.
                FieldList = obj as List<Field>;
                //FieldList = (List<Field>)obj;

                ListFields();
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            Hepsi.Clear();

            Form frm = new Form();
            frm.Width = 500;
            frm.Height = 330;
            frm.FormBorderStyle = FormBorderStyle.FixedSingle;
            frm.MaximizeBox = false;
            frm.Text = "Veri Giriş Formu";
            frm.FormClosing += Frm_FormClosing;
            frm.Load += Frm_Load;

            SplitContainer sc = new SplitContainer();
            sc.Dock = DockStyle.Fill;   // SlitContainer'ı form'a yayar.
            frm.Controls.Add(sc);       // Form içine kontrol ekleme.

            FlowLayoutPanel flp = new FlowLayoutPanel();
            flp.FlowDirection = FlowDirection.RightToLeft;
            flp.Dock = DockStyle.Bottom;
            flp.Height = 60;
            flp.Padding = new Padding(8);

            Button btn_Sil = new Button();
            btn_Sil.Text = "Sil";
            flp.Controls.Add(btn_Sil);
            btn_Sil.Click += Btn_Sil_Click;

            Button btn_Guncelle = new Button();
            btn_Guncelle.Text = "Güncelle";
            flp.Controls.Add(btn_Guncelle);
            btn_Guncelle.Click += Btn_Guncelle_Click;

            Button btn_Ekle = new Button();
            btn_Ekle.Text = "Ekle";
            flp.Controls.Add(btn_Ekle);
            btn_Ekle.Click += Btn_Ekle_Click;

            ListBox lst = new ListBox();
            lst.Name = "lstItems";
            lst.Dock = DockStyle.Fill;
            lst.SelectedIndexChanged += Lst_SelectedIndexChanged;

            sc.Panel1.Controls.Add(lst);

            FlowLayoutPanel flpFields = new FlowLayoutPanel();
            flpFields.Dock = DockStyle.Fill;
            flpFields.AutoScroll = true;
            flpFields.Name = "flpFields";

            flpFields.FlowDirection = FlowDirection.TopDown;

            sc.Panel2.Controls.Add(flpFields);
            sc.Panel2.Controls.Add(flp);

            foreach (Field item in FieldList)
            {
                Label lbl = new Label();
                lbl.Padding = new Padding(8, 5, 8, 3);
                lbl.Text = item.FieldName;
                lbl.Font = new Font("Tahoma", 9, FontStyle.Bold);
                lbl.ForeColor = Color.DarkRed;
                lbl.Margin = new Padding(0, 3, 30, 0);

                flpFields.Controls.Add(lbl);

                switch (item.FieldType)
                {
                    case "Text":
                        TextBox txt = new TextBox();
                        txt.Name = "txt_" + item.FieldName;
                        txt.Width = 180;
                        txt.Margin = new Padding(0, 3, 30, 0);

                        flpFields.Controls.Add(txt);
                        break;

                    case "Number":
                        NumericUpDown nud = new NumericUpDown();
                        nud.Name = "nud_" + item.FieldName;
                        nud.Width = 180;
                        nud.Margin = new Padding(0, 3, 30, 0);

                        flpFields.Controls.Add(nud);
                        break;

                    case "Date":
                        DateTimePicker dtp = new DateTimePicker();
                        dtp.Name = "dtp_" + item.FieldName;
                        dtp.Format = DateTimePickerFormat.Short;
                        dtp.Width = 180;
                        dtp.Margin = new Padding(0, 3, 30, 0);

                        flpFields.Controls.Add(dtp);
                        break;

                    case "Check":
                        CheckBox chk = new CheckBox();
                        chk.Name = "chk_" + item.FieldName;
                        chk.Text = item.FieldName;
                        chk.Margin = new Padding(0, 3, 30, 0);

                        flpFields.Controls.Add(chk);
                        break;

                    default:
                        break;
                }
            }

            frm.ShowDialog();
        }

        private void Frm_Load(object sender, EventArgs e)
        {
            if(File.Exists(JsonDataFilePath))
            {
                Form frm = sender as Form;

                Json js = new Json();
                object obj = js.Deserialize(JsonDataFilePath, typeof(List<Dictionary<string, string>>));

                Hepsi = obj as List<Dictionary<string, string>>;
                HepsiListele(frm);
            }
        }

        private void Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lst = sender as ListBox;
            Form frm = lst.FindForm();

            if (lst.SelectedIndex > -1)
            {
                Dictionary<string, string> item = Hepsi[lst.SelectedIndex];
                FlowLayoutPanel flp = frm.Controls.Find("flpFields", true)[0] as FlowLayoutPanel;

                foreach (string key in item.Keys)
                {
                    Control[] controls = flp.Controls.Find(key, false);

                    if (controls.Length > 0)
                    {
                        Control c = controls[0];

                        if (c is TextBox)
                        {
                            TextBox txt = (TextBox)c;
                            txt.Text = item[key];
                        }
                        else if (c is DateTimePicker)
                        {
                            DateTimePicker dtp = (DateTimePicker)c;
                            dtp.Value = DateTime.Parse(item[key]);
                        }
                        else if (c is NumericUpDown)
                        {
                            NumericUpDown nud = (NumericUpDown)c;
                            nud.Value = decimal.Parse(item[key]);
                        }
                        else if (c is CheckBox)
                        {
                            CheckBox chk = (CheckBox)c;
                            chk.Checked = bool.Parse(item[key]);
                        }
                    }
                }
            }
        }

        private void Frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Json js = new Json();
            js.Serialize(JsonDataFilePath, Hepsi);
        }

        private void Btn_Ekle_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Form frm = btn.FindForm();
            Control[] panels = frm.Controls.Find("flpFields", true);

            if (panels.Length > 0)
            {
                FlowLayoutPanel flp = (FlowLayoutPanel)panels[0];
                Dictionary<string, string> data = new Dictionary<string, string>();

                foreach (Control item in flp.Controls)
                {
                    if (item is Label == false)
                    {
                        if (item is TextBox)
                        {
                            TextBox txt = (TextBox)item;
                            data.Add(txt.Name, txt.Text);
                        }
                        else if (item is DateTimePicker)
                        {
                            DateTimePicker dtp = (DateTimePicker)item;
                            data.Add(dtp.Name, dtp.Value.ToShortDateString());
                        }
                        else if (item is NumericUpDown)
                        {
                            NumericUpDown nud = (NumericUpDown)item;
                            data.Add(nud.Name, nud.Value.ToString());
                        }
                        else if (item is CheckBox)
                        {
                            CheckBox chk = (CheckBox)item;
                            data.Add(chk.Name, chk.Checked.ToString());
                        }
                    }
                }

                Hepsi.Add(data);
                HepsiListele(frm);
            }
        }

        private void HepsiListele(Form f)
        {
            ListBox lst = f.Controls.Find("lstItems", true)[0] as ListBox;
            lst.Items.Clear();

            foreach (Dictionary<string,string> item in Hepsi)
            {
                lst.Items.Add(item[item.Keys.First()]);
            }
        }

        private void Btn_Guncelle_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Form frm = btn.FindForm();
            ListBox lst = frm.Controls.Find("lstItems", true)[0] as ListBox;
            Control[] panels = frm.Controls.Find("flpFields", true);

            if (lst.SelectedIndex < 0)
            {
                return;
            }

            if (panels.Length > 0)
            {
                FlowLayoutPanel flp = (FlowLayoutPanel)panels[0];
                Dictionary<string, string> data = new Dictionary<string, string>();

                foreach (Control item in flp.Controls)
                {
                    if (item is Label == false)
                    {
                        if (item is TextBox)
                        {
                            TextBox txt = (TextBox)item;
                            data.Add(txt.Name, txt.Text);
                        }
                        else if (item is DateTimePicker)
                        {
                            DateTimePicker dtp = (DateTimePicker)item;
                            data.Add(dtp.Name, dtp.Value.ToShortDateString());
                        }
                        else if (item is NumericUpDown)
                        {
                            NumericUpDown nud = (NumericUpDown)item;
                            data.Add(nud.Name, nud.Value.ToString());
                        }
                        else if (item is CheckBox)
                        {
                            CheckBox chk = (CheckBox)item;
                            data.Add(chk.Name, chk.Checked.ToString());
                        }
                    }
                }

                Hepsi[lst.SelectedIndex] = data;
                HepsiListele(frm);
            }
        }

        private void Btn_Sil_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Form frm = btn.FindForm();
            Control[] controls = frm.Controls.Find("lstItems", true);

            if (controls.Length > 0)
            {
                ListBox lst = controls[0] as ListBox;

                Hepsi.RemoveAt(lst.SelectedIndex);
                HepsiListele(frm);
            }
        }
    }
}
