using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace PredvidjanjeZavrsneOcjene
{
    
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            studentData data1 = new studentData();
            data1.studyTime = textBox1.Text;
            data1.failures = textBox2.Text;
            data1.absences = textBox3.Text;
            data1.G1 = textBox4.Text;
            data1.G2 = textBox5.Text;
           InvokeRequestResponseService(data1, label6).Wait();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (int.Parse(textBox1.Text) > 4)
            {
                textBox1.Text = "4";
            }
            else if (int.Parse(textBox1.Text) < 1)
            {
                textBox1.Text = "1";
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (int.Parse(textBox2.Text) > 3)
            {
                textBox2.Text = "3";
            }
            else if (int.Parse(textBox2.Text) < 0)
            {
                textBox2.Text = "0";
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (int.Parse(textBox3.Text) > 93)
            {
                textBox3.Text = "93";
            }
            else if (int.Parse(textBox3.Text) < 0)
            {
                textBox3.Text = "0";
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (int.Parse(textBox4.Text) > 20)
            {
                textBox4.Text = "20";
            }
            else if (int.Parse(textBox4.Text) < 0)
            {
                textBox4.Text = "0";
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (int.Parse(textBox5.Text) > 20)
            {
                textBox5.Text = "20";
            }
            else if (int.Parse(textBox5.Text) < 0)
            {
                textBox5.Text = "0";
            }
        }

        static async Task InvokeRequestResponseService(studentData data, Label label6)
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {

                    Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"studytime", "failures", "absences", "G1", "G2"},
                                Values = new string[,]  { { data.studyTime, data.failures, data.absences, data.G1, data.G2 } }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };
                const string apiKey = "RKY6CqvztAZBxu2vydpYPTowIhKGFoxRDYBlrEG0ZYJEGhoSXxeK40C8lLRBEB/EwJIXdVfNeNqSUd2BDo1Tjg=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/f5738dc50f4e466db5c70f5df304735a/" +
                    "services/338b6c6c58b443608004bc258cb188c7/execute?api-version=2.0&details=true");

                // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)


                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest).ConfigureAwait(false);
                Form1 form = new Form1();
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    dynamic studentData = JObject.Parse(result);

                    label6.Text = "Predicted final grade (G3): " + studentData.Results.output1.value.Values[0][5];
                }
                else
                {
                    label6.Text = "The request failed with status code: {0}" + response.StatusCode;
                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                    label6.Text += "\n " + response.Headers.ToString();
                    string responseContent = await response.Content.ReadAsStringAsync();
                    label6.Text += "\n " + responseContent;
                }
            }
        }
    }
    public class studentData
    {
        public string studyTime;
        public string failures;
        public string absences;
        public string G1;
        public string G2;
    }

    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }
    
}
