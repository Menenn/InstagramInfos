using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Interactions;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace InstagramInfos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            listView3.DoubleClick += doubleClick3;
        }


        public void doubleClick3(object sender, EventArgs e)
        {
            string script = "window.open('https://www.instagram.com/" + listView3.SelectedItems[0].Text + "', '_blank');";
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
            jsExecutor.ExecuteScript(script);
        }

        public IWebDriver driver;
        async void rodar()
        {
            // Credenciais do Instagram
            try
            {
                try
                {
                    comandoJS(@"const divToDelete = document.getElementById('followers');

                    if (divToDelete)
                    {
                        divToDelete.remove();
                    }");
                    comandoJS(@"const divToDelete = document.getElementById('followings');

                    if (divToDelete)
                    {
                        divToDelete.remove();
                    }");
                    comandoJS(@"const divToDelete = document.getElementById('dontFollowMeBack');

                    if (divToDelete)
                    {
                        divToDelete.remove();
                    }");
                    comandoJS(@"const divToDelete = document.getElementById('iDontFollowBack');

                    if (divToDelete)
                    {
                        divToDelete.remove();
                    }");

                    IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                    string javascriptCode = @"const username = '" + textBox2.Text + @"';

let followers = [{ username: "", full_name: "" }];
let followings = [{ username: "", full_name: "" }];
let dontFollowMeBack = [{ username: "", full_name: "" }];
let iDontFollowBack = [{ username: "", full_name: "" }];

followers = [];
followings = [];
dontFollowMeBack = [];
iDontFollowBack = [];

(async () => {
  try {

    const userQueryRes = await fetch(
      `https://www.instagram.com/web/search/topsearch/?query=${username}`
    );

    const userQueryJson = await userQueryRes.json();

    const userId = userQueryJson.users[0].user.pk;

    let after = null;
    let has_next = true;

    while (has_next) {
      await fetch(
        `https://www.instagram.com/graphql/query/?query_hash=c76146de99bb02f6415203be841dd25a&variables=` +
          encodeURIComponent(
            JSON.stringify({
              id: userId,
              include_reel: true,
              fetch_mutual: true,
              first: 50,
              after: after,
            })
          )
      )
        .then((res) => res.json())
        .then((res) => {
          has_next = res.data.user.edge_followed_by.page_info.has_next_page;
          after = res.data.user.edge_followed_by.page_info.end_cursor;
          followers = followers.concat(
            res.data.user.edge_followed_by.edges.map(({ node }) => {
              return {
                username: node.username,
                full_name: node.full_name,
              };
            })
          );
        });
    }

    console.log({ followers });
    createInfoDiv('followers', followers);

    after = null;
    has_next = true;

    while (has_next) {
      await fetch(
        `https://www.instagram.com/graphql/query/?query_hash=d04b0a864b4b54837c0d870b0e77e076&variables=` +
          encodeURIComponent(
            JSON.stringify({
              id: userId,
              include_reel: true,
              fetch_mutual: true,
              first: 50,
              after: after,
            })
          )
      )
        .then((res) => res.json())
        .then((res) => {
          has_next = res.data.user.edge_follow.page_info.has_next_page;
          after = res.data.user.edge_follow.page_info.end_cursor;
          followings = followings.concat(
            res.data.user.edge_follow.edges.map(({ node }) => {
              return {
                username: node.username,
                full_name: node.full_name,
              };
            })
          );
        });
    }

    console.log({ followings });
    createInfoDiv('followings', followings);

    dontFollowMeBack = followings.filter((following) => {
      return !followers.find(
        (follower) => follower.username === following.username
      );
    });

    console.log({ dontFollowMeBack });
    createInfoDiv('dontFollowMeBack', dontFollowMeBack);

    iDontFollowBack = followers.filter((follower) => {
      return !followings.find(
        (following) => following.username === follower.username
      );
    });

    console.log({ iDontFollowBack });
    createInfoDiv('iDontFollowBack', iDontFollowBack);
    
    console.log(
      `Process is done: Type 'copy(followers)' or 'copy(followings)' or 'copy(dontFollowMeBack)' or 'copy(iDontFollowBack)' in the console and paste it into a text editor to take a look at it'`
    );
  } catch (err) {
    console.log({ err });
  }
})();

function createInfoDiv(id, data) {
  const div = document.createElement('div');
  div.id = id;

  if (data.length > 0) {
    data.forEach((item) => {
      const p = document.createElement('p');
      p.textContent = JSON.stringify(item);
      div.appendChild(p);
    });
  } else {
    const p = document.createElement('p');
    p.textContent = 'No data available';
    div.appendChild(p);
  }

  document.body.appendChild(div);
}";

                    // Execute o código JavaScript e aguarde sua conclusão
                    //textBox1.Text += ("Executando Script: " + javascriptCode) + "\n";
                    string jsResult = (string)jsExecutor.ExecuteScript(
                        javascriptCode
                    );

                    //MessageBox.Show(javascriptCode);

                    //Console.WriteLine("Resultado do JavaScript: " + jsResult);
                    bool resultsok = false;
                    while (!resultsok)
                    {
                        string html = driver.PageSource;
                        if (html.Contains("div id=\"followers\"") && html.Contains("div id=\"followings\"") && html.Contains("div id=\"dontFollowMeBack\"") && html.Contains("div id=\"iDontFollowBack\""))
                        {
                            resultsok = true;
                        }
                        Thread.Sleep(5000);
                        textBox1.Text += ("HTML: " + resultsok) + " - \n";
                    }

                    if (resultsok)
                    {
                        Processar();
                    }
                    //textBox1.Text += ("Resultado: " + jsResult) + "\n";
                }
                catch (Exception ex)
                {
                    textBox1.Text += ("Erro: " + ex.Message) + "\n";
                }
                finally
                {
                    //driver.Quit();
                }
            }
            catch (Exception ex)
            {
                textBox1.Text += ("Erro: " + ex.Message) + "\n";
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Por favor utilize o site Instagram.com para realizar o login, depois retorne e clique em 'Pesquisar'");
            }
            catch (Exception ex)
            {
                textBox1.Text += ("Erro: " + ex.Message) + "\n";
            }

            AbrirJanela();
        }

        void AbrirJanela()
        {
            string chromePath = AppDomain.CurrentDomain.BaseDirectory + @"\Chrome\Chrome.exe";

            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.BinaryLocation = chromePath;
            chromeOptions.AddArgument($"--disk-cache-dir={AppDomain.CurrentDomain.BaseDirectory + @"\Chrome\Cache"}");
            chromeOptions.AddArgument($"--profile-directory={AppDomain.CurrentDomain.BaseDirectory + @"\Chrome\Profile"}");
            chromeOptions.AddArgument($"--user-data-dir={AppDomain.CurrentDomain.BaseDirectory + @"\Chrome\Data"}");

            driver = new ChromeDriver(chromeOptions);

            driver.Navigate().GoToUrl("https://www.instagram.com/");
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox2.Text != "" && textBox2.Text.Length > 3)
                {
                    rodar();
                }
                else
                {
                    MessageBox.Show("Por favor insira um nome de usuario valido!");
                }
            }
            catch (Exception ex)
            {

            }
        }

        public string followersRaw = "";
        public string followingsRaw = "";
        public string dontFollowMeBackRaw = "";
        public string iDontFollowBackRaw = "";

        public List<infosInsta> followers = new List<infosInsta>();
        public List<infosInsta> followings = new List<infosInsta>();
        public List<infosInsta> dontFollowMeBack = new List<infosInsta>();
        public List<infosInsta> iDontFollowBack = new List<infosInsta>();
        private void button3_Click(object sender, EventArgs e)
        {
            Processar();
        }

        public void Processar()
        {
            try
            {
                followersRaw = coletarHTML("const followersInfoHTML = document.getElementById('followers').outerHTML; return followersInfoHTML;");
                followingsRaw = coletarHTML("const followingsInfoHTML = document.getElementById('followings').outerHTML; return followingsInfoHTML;");
                dontFollowMeBackRaw = coletarHTML("const dontFollowMeBackInfoHTML = document.getElementById('dontFollowMeBack').outerHTML; return dontFollowMeBackInfoHTML;");
                iDontFollowBackRaw = coletarHTML("const iDontFollowBackInfoHTML = document.getElementById('iDontFollowBack').outerHTML; return iDontFollowBackInfoHTML;");

                followers = ProcessarLista(followersRaw);
                followings = ProcessarLista(followingsRaw);
                dontFollowMeBack = ProcessarLista(dontFollowMeBackRaw);
                iDontFollowBack = ProcessarLista(iDontFollowBackRaw);

                textBox1.Text += ("Numero Seguidores: " + followers.Count) + "\n";
                textBox1.Text += ("Seguidores: ") + "\n";

                listView1.Items.Clear();
                listView2.Items.Clear();
                listView3.Items.Clear();
                listView4.Items.Clear();
                foreach (var follow in followers)
                {
                    ListViewItem itemadd = new ListViewItem();
                    itemadd.Text = follow.ID;
                    itemadd.SubItems.Add(follow.Name);
                    listView1.Items.Add(itemadd);
                    //textBox1.Text += (follow.ID + " - " + follow.Name + " . ") + "\n";
                }

                foreach (var follow in followings)
                {
                    ListViewItem itemadd = new ListViewItem();
                    itemadd.Text = follow.ID;
                    itemadd.SubItems.Add(follow.Name);
                    listView2.Items.Add(itemadd);
                    //textBox1.Text += (follow.ID + " - " + follow.Name + " . ") + "\n";
                }

                foreach (var follow in dontFollowMeBack)
                {
                    ListViewItem itemadd = new ListViewItem();
                    itemadd.Text = follow.ID;
                    itemadd.SubItems.Add(follow.Name);
                    listView3.Items.Add(itemadd);
                    //textBox1.Text += (follow.ID + " - " + follow.Name + " . ") + "\n";
                }

                foreach (var follow in iDontFollowBack)
                {
                    ListViewItem itemadd = new ListViewItem();
                    itemadd.Text = follow.ID;
                    itemadd.SubItems.Add(follow.Name);
                    listView4.Items.Add(itemadd);
                    //textBox1.Text += (follow.ID + " - " + follow.Name + " . ") + "\n";
                }

                toolStripMenuItem1.Text = "Seguidores (" + followers.Count + ")";
                toolStripMenuItem2.Text = "Seguindo (" + followings.Count + ")";
                toolStripMenuItem3.Text = "Nao seguem de volta (" + dontFollowMeBack.Count + ")";
                toolStripMenuItem4.Text = "Voce nao segue de volta (" + iDontFollowBack.Count + ")";

                //textBox1.Text += ("SeguidoresRaw: " + followersRaw) + "\n";
                //textBox1.Text += ("Seguidores: " + followersRaw) + "\n";
                //textBox1.Text += ("Seguindo: " + followingsRaw) + "\n";
            }
            catch (Exception ex)
            {
                textBox1.Text += "Erro Logs: " + ex.Message;
            }
        }

        public class infosInsta
        {
            public string ID;
            public string Name;
        }

        public List<infosInsta> ProcessarLista(string conteudo)
        {
            List<infosInsta> listaretornar = new List<infosInsta>();
            Regex regex = new Regex(@"{""username"":""(.*?)"",""full_name"":""(.*?)""}");

            // Usamos Matches para encontrar todas as correspondências na entrada
            MatchCollection matches = regex.Matches(conteudo);

            foreach (Match match in matches)
            {
                infosInsta add = new infosInsta();
                add.ID = match.Groups[1].Value;
                add.Name = match.Groups[2].Value;

                listaretornar.Add(add);
            }
            return listaretornar;
        }

        string coletarHTML(string codigo)
        {
            string result = "";

            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;

            result = (string)jsExecutor.ExecuteScript(
                codigo
            );

            return result;
        }

        void comandoJS(string codigo)
        {
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;

            jsExecutor.ExecuteScript(
                codigo
            );
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OffAll();
            OnAll();
            listView1.Visible = true;
            toolStripMenuItem1.Enabled = false;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            OffAll();
            OnAll();
            listView2.Visible = true;
            toolStripMenuItem2.Enabled = false;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            OffAll();
            OnAll();
            listView3.Visible = true;
            toolStripMenuItem3.Enabled = false;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            OffAll();
            OnAll();
            listView4.Visible = true;
            toolStripMenuItem4.Enabled = false;
        }

        void OffAll()
        {
            listView1.Visible = false;
            listView2.Visible = false;
            listView3.Visible = false;
            listView4.Visible = false;
        }

        void OnAll()
        {
            toolStripMenuItem1.Enabled = true;
            toolStripMenuItem2.Enabled = true;
            toolStripMenuItem3.Enabled = true;
            toolStripMenuItem4.Enabled = true;
        }
    }
}
