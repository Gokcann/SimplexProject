using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace simplex_proje
{
    class Program
    {
        static void Main(string[] args)
        {
            string amacfonkisim = "";


            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\vaio\Desktop\ornek.mps");
            // burada .mps dosyasını satır satır okuyup string diziye atıyoruz
            int denklemlersayac = 0, degiskensayac = 0;
            int rowsatır = 0, columssatır = 0, rhssatır = 0, boundssatır = 0, enddatasatır = 0;
            string sepRegex = " |, ";

            Regex rx = new Regex(sepRegex);
            for (int j = 0; j < lines.Length; j++)
            {
                string[] words = rx.Split(lines[j]);
                //burada satırdaki her kelimeyi ayırıyoruz
                for (int k = 0; k < words.Length; k++)
                {

                    //aşağıda problem için gerekli olan başlıca özelliklerin yerlerini buluyoruz
                    if (words[k] == "ROWS")
                    {
                        rowsatır = j;
                        Console.WriteLine(words[k] + " " + rowsatır);
                    }
                    if (words[k] == "COLUMNS")
                    {
                        columssatır = j;
                        Console.WriteLine(words[k] + " " + columssatır);
                    }
                    if (words[k] == "RHS")
                    {
                        rhssatır = j;
                        Console.WriteLine(words[k] + " " + rhssatır);
                    }
                    if (words[k] == "BOUNDS")
                    {
                        boundssatır = j;
                        Console.WriteLine(words[k] + " " + boundssatır);
                    }
                    if (words[k] == "ENDATA")
                    {
                        enddatasatır = j;
                        Console.WriteLine(words[k] + " " + enddatasatır);
                    }
                    /*    if (words[k] == "RANGE")
                        {
                            enddatasatır = j;
                            Console.WriteLine(words[k] + " " + enddatasatır);
                        }*/
                }
            }
            string[] denklemad = new string[columssatır - rowsatır - 1 - 1];
            int[] eklenicekler = new int[columssatır - rowsatır - 1 - 1];
            string[] denklemtur = new string[columssatır - rowsatır - 1 - 1];
            int rsayac = 0, toplamsayi = 0;

            Console.WriteLine(lines[3].Length);
            for (int i = rowsatır; i < columssatır; i++)
            {
                //burada row ile colums satırları arasını geziyoruz
                if (lines[i].Substring(1, 1) == "N")//amac fonksiyonunun adı
                {
                    amacfonkisim = lines[i].Substring(4, lines[i].Length - 4);


                }
                if (lines[i].Substring(1, 1) == "L")//kucuk esittir fonksiyonlarının isimlerini tutuyoruz
                {
                    denklemtur[denklemlersayac] = "L";
                    denklemad[denklemlersayac] = lines[i].Substring(4, lines[i].Length - 4);

                    eklenicekler[denklemlersayac] = 1;//hangi denkleme kac tane degisken ekleyecegimizi tutuyoruz
                    denklemlersayac++;//toplam denklem sayisini tutuyoruz
                    toplamsayi++;//eklenen toplam degisken sayisini tutuyoruz

                }
                if (lines[i].Substring(1, 1) == "G")//buyuk esittir fonksiyonlarının isimlerini tutuyoruz
                {
                    denklemtur[denklemlersayac] = "G";
                    denklemad[denklemlersayac] = lines[i].Substring(4, lines[i].Length - 4);

                    eklenicekler[denklemlersayac] = 2;
                    rsayac++;//eklenen r sayisini tutuyoruz
                    denklemlersayac++;
                    toplamsayi += 2;
                }
                if (lines[i].Substring(1, 1) == "E")//esittir fonksiyonlarının isimlerini tutuyoruz
                {
                    denklemtur[denklemlersayac] = "E";
                    denklemad[denklemlersayac] = lines[i].Substring(4, lines[i].Length - 4);

                    eklenicekler[denklemlersayac] = 1;
                    rsayac++;
                    denklemlersayac++;
                    toplamsayi++;
                }

            }


            foreach (string s in denklemad)//denklemlerin adlarini yazdiriyoruz
                Console.WriteLine(s);

            string[] gecici = new string[rhssatır - columssatır - 1];
            Console.WriteLine(gecici.Length);
            gecici[degiskensayac] = lines[columssatır + 1].Substring(4, 7);
            degiskensayac++;
            for (int i = columssatır + 2; i < rhssatır; i++)//colums satir ile rhs satiri arasini geziyoruz
            {
                if (lines[i - 1].Substring(4, 7) != lines[i].Substring(4, 7))
                {
                    gecici[degiskensayac] = lines[i].Substring(4, 7);//degiskenlerimizi aliyoruz
                    degiskensayac++;
                }
            }
            string[] degiskenad = new string[degiskensayac];
            for (int i = 0; i < degiskensayac; i++)
                degiskenad[i] = gecici[i];//degisken isimlerini degisken sayisi kadar ifadede tutuyoruz
            Console.WriteLine(degiskensayac);
            double[,] degiskendeger = new double[denklemlersayac, degiskensayac];
            //degisken degerlerini tutacagimiz iki boyutlu dizimiz
            double[] amacfonkdeger = new double[degiskensayac];
            //amac fonksiyonunun degerlerini tutacagimiz dizi



            for (int i = 0; i < lines.Length; i++)//.mps dosyasindan okuduğumuz tum noktali(.) ifadeleri virgullu(,) hale ceviriyoruz
            {
                lines[i] = lines[i].Replace(".", ",");
            }

            for (int i = columssatır + 1; i < rhssatır; i++)//denklemdeki degisken katsayilarini alacagimiz kisim
            {
                for (int j = 0; j < degiskensayac; j++)
                {
                    if (lines[i].Substring(4, 7) == degiskenad[j])//denklem isimlerine göre aliyoruz
                    {
                        if (lines[i].Substring(14, amacfonkisim.Length) == amacfonkisim)//satirdaki 15 ve amac fonksiyonunun uzunlugu kadar kismi alip ismi ile karsilastiriyor
                        {
                            amacfonkdeger[j] = Convert.ToDouble(lines[i].Substring(24, 12));

                            //doğru ise sonraki asamada amac fonksiyonunun degerini 25 ve 37 karakter arasindan aliyor
                        }
                        else for (int a = 0; a < denklemlersayac; a++)//amac fonksiyonu degilse denklem ismi ile karsilastirip sonraki degeri alanina uygun sekilde aliyoruz
                            {
                                if (lines[i].Substring(14, denklemad[a].Length) == denklemad[a])
                                {
                                    degiskendeger[a, j] = Convert.ToDouble(lines[i].Substring(24, 12));
                                }
                            }
                        // 5. ve 6. alan için bakıyoruz
                        if (lines[i].Length > 36)
                        {
                            if (lines[i].Substring(39, amacfonkisim.Length) == amacfonkisim)
                            {
                                amacfonkdeger[j] = Convert.ToDouble(lines[i].Substring(49, 12));
                                Console.WriteLine(amacfonkdeger[j]);
                            }
                            else for (int a = 0; a < denklemlersayac; a++)
                                {
                                    if (lines[i].Substring(39, denklemad[a].Length) == denklemad[a])
                                    {
                                        degiskendeger[a, j] = Convert.ToDouble(lines[i].Substring(49, 12));
                                        Console.WriteLine(degiskendeger[a, j]);
                                    }
                                }
                        }
                    }
                }

            }
            double[] yedekamacfonkdeger = new double[degiskensayac + toplamsayi];//amac fonksiyonunu kaybetmemek icin yedegini aliyoruz
            for (int i = 0; i < amacfonkdeger.Length; i++)
                yedekamacfonkdeger[i] = amacfonkdeger[i];
            double[] denklemdeger = new double[denklemlersayac];

            for (int i = rhssatır + 1; i < boundssatır; i++)//denklemlerin degerlerini aliyoruz
            {
                for (int j = 0; j < denklemlersayac; j++)
                {
                    if (lines[i].Substring(14, denklemad[j].Length) == denklemad[j])//denklemlerin adina gore 
                    {
                        denklemdeger[j] = Convert.ToDouble(lines[i].Substring(24, 12));
                    }
                    if (lines[i].Length > 36)
                    {
                        if (lines[i].Substring(39, denklemad[j].Length) == denklemad[j])
                        {
                            denklemdeger[j] = Convert.ToDouble(lines[i].Substring(49, 12));
                        }
                    }
                }

            }
            double[] degiskenüstdeger = new double[degiskensayac];//bunları aliyoruz fakat kullanmıyoruz
            double[] degiskenaltdeger = new double[degiskensayac];
            for (int b = 0; b < degiskenad.Length; b++)//tüm line satırlarındaki boşlukları sildiriyoruz
            {
                string numara = "";
                string[] a = degiskenad[b].Split(' ');
                for (int i = 0; i < a.Length; i++)
                {
                    numara += a[i];

                }
                degiskenad[b] = numara;
            }//silme islemi burada bitiyor

            for (int i = boundssatır + 1; i < enddatasatır; i++)//bu satirlari aliyoruz fakat kullanmiyoruz
            {
                for (int j = 0; j < degiskensayac; j++)
                {
                    if (lines[i].Substring(1, 2) == "UP")
                    {
                        if (lines[i].Substring(14, degiskenad[j].Length) == degiskenad[j])
                        {
                            degiskenüstdeger[j] = Convert.ToDouble(lines[i].Substring(24, 12));
                            Console.WriteLine(degiskenüstdeger[j]);
                        }
                    }
                    if (lines[i].Substring(1, 2) == "LO")
                    {
                        if (lines[i].Substring(14, degiskenad[j].Length) == degiskenad[j])
                        {
                            degiskenaltdeger[j] = Convert.ToDouble(lines[i].Substring(24, 12));
                            Console.WriteLine(degiskenaltdeger[j]);
                        }
                    }
                    if (lines[i].Substring(1, 2) == "FR")
                    {
                        if (lines[i].Substring(14, degiskenad[j].Length) == degiskenad[j])
                        {
                            //free degisken ama kullanmadigimiz icin ne yapacagimizi bilemedik
                        }
                    }
                }
            }
            int[] denklemekeklenecekdegiskensayisi = new int[degiskensayac];

            StreamWriter altüstyazıcı = File.AppendText(@"C:\Users\vaio\Desktop\altüst.txt");
            //kullanmadigimiz alt ust degeri .txt dosyasina kaydediyoruz
            foreach (double s in degiskenüstdeger)
            {
                altüstyazıcı.Write(s);
                altüstyazıcı.Write("*");
            }
            altüstyazıcı.WriteLine();
            foreach (double s in degiskenaltdeger)
            {
                altüstyazıcı.Write(s);
                altüstyazıcı.Write("*");
            }

            altüstyazıcı.Close();
            StreamWriter standarthal = File.AppendText(@"C:\Users\vaio\Desktop\standarthal.txt");
            //standart hale getirme islemimiz burada basliyor
            int rsayackont = 0, ssayackont = 0, kntrl = 0;
            int[] hangisi = new int[denklemlersayac];
            for (int x = 0; x < denklemlersayac; x++) // standart hale getirme
            {
                kntrl = 0;
                for (int y = 0; y < degiskensayac + 1; y++)
                {
                    if (degiskensayac > y)//burada degiskenlerin degerlerini yaziyor
                    {
                        standarthal.Write(degiskendeger[x, y] + "*");//her ekledigimiz degiskenden sonra * karakteri ile ayiriyoruz
                        kntrl++;
                    }
                    if (degiskensayac <= y)//degisken sayisi bittikten sonra denklemin turune göre (<=,>=,=) yapay ve dolgu degiskenlerimizi ekliyoruz
                    {
                        if (denklemtur[x] == "L")
                        {
                            for (int i = 0; i < rsayac; i++)//buradaki denklemin degiskenlerinden sonra daha once kac adet r'miz var ise o kadar 0 yaziyoruz 
                            {
                                standarthal.Write("0" + "*");
                                kntrl++;
                            }

                            for (int i = 0; i < ssayackont; i++)//onceki tum s'leri yazdiriyoruz
                            {
                                kntrl++;
                                standarthal.Write("0" + "*");
                            }
                            standarthal.Write("1" + "*");//su an bulundugumuz s konumu
                            ssayackont++;
                            kntrl++;
                            hangisi[x] = kntrl;//giren degisenimizi aliyoruz

                            for (int i = 0; i < (toplamsayi - rsayac) - ssayackont; i++)
                            {
                                standarthal.Write("0" + "*");//buradaki denklemin degiskenlerinden sonra daha sonra kac adet s'miz var ise o kadar 0 yaziyoruz
                            }
                            standarthal.WriteLine();
                        }
                        if (denklemtur[x] == "G")
                        {
                            for (int i = 0; i < rsayackont; i++)
                            {
                                standarthal.Write("0" + "*");
                                kntrl++;
                            }
                            kntrl++;
                            hangisi[x] = kntrl;
                            standarthal.Write("1" + "*");
                            rsayackont++;
                            for (int i = 0; i < (rsayac - rsayackont); i++)
                            {
                                standarthal.Write("0" + "*");

                            }
                            for (int i = 0; i < ssayackont; i++)
                            {

                                standarthal.Write("0" + "*");
                            }
                            standarthal.Write("-1" + "*");
                            ssayackont++;

                            for (int i = 0; i < (toplamsayi - rsayac) - ssayackont; i++)
                            {
                                standarthal.Write("0" + "*");
                            }
                            standarthal.WriteLine();
                        }
                        if (denklemtur[x] == "E")
                        {
                            for (int i = 0; i < rsayackont; i++)
                            {
                                standarthal.Write("0" + "*");
                                kntrl++;
                            }
                            kntrl++;
                            hangisi[x] = kntrl;
                            standarthal.Write("1" + "*");
                            rsayackont++;
                            for (int i = 0; i < (rsayac - rsayackont); i++)
                            {
                                standarthal.Write("0" + "*");

                            }

                            for (int i = 0; i < ssayackont; i++)
                            {

                                standarthal.Write("0" + "*");
                            }

                            for (int i = 0; i < (toplamsayi - rsayac) - ssayackont; i++)
                            {
                                standarthal.Write("0" + "*");
                            }
                            standarthal.WriteLine();
                        }
                    }
                }
            }
            for (int i = 0; i < degiskensayac + toplamsayi; i++)//amac fonksiyonunu .txt dosyasina yazdiriyoruz 
            {
                if (i < degiskensayac)
                {
                    standarthal.Write("0" + "*");
                }
                else if (i < degiskensayac + rsayac)
                {
                    standarthal.Write("1" + "*");
                }
                else
                    standarthal.Write("0" + "*");
            }

            /*   foreach (double s in amacfonkdeger)
                   standarthal.Write(s + "*");
               standarthal.WriteLine();*/

            //      TextReader okuyucu = new StreamReader(@"C:\Users\vaio\Desktop\altüst.txt");
            // foreach (string s in okuyucu)
            //       Console.WriteLine(okuyucu.ReadLine());



            /*   string metin ;
               while ((metin = okuyucu.ReadLine()) != null)
   {
   Console.WriteLine(metin);
   }
   */
            //okuyucu.Close();

            standarthal.Close();
            string[] satir = System.IO.File.ReadAllLines(@"C:\Users\vaio\Desktop\standarthal.txt");//standart hale getirdigimiz veriyi okumak icin bu islemi yapiyoruz
            double[,] simp = new double[satir.Length, degiskensayac + toplamsayi];
            char[] yıldız = { '*' };
            for (int i = 0; i < satir.Length; i++)
            {
                string[] words = satir[i].Split(yıldız);//yildiz karakterine göre ayiriyoruz
                for (int j = 0; j < words.Length; j++)
                {


                    if (words[j] != "")//satirdaki her kelimeyi sirasiyla okuyoruz
                        simp[i, j] = Convert.ToDouble(words[j]);
                }
            }





            int yerbul = 0;
            double geciciamacfonkdeger = 0;
            for (int i = 0; i < denklemlersayac; i++)
            {

                yerbul = hangisi[i] - 1;
                for (int j = 0; j < degiskensayac + toplamsayi; j++)
                {

                    simp[i, j] = simp[i, j] / simp[i, yerbul];

                }
            }
            for (int i = 0; i < denklemlersayac; i++)
            {

                yerbul = hangisi[i] - 1;

                for (int j = 0; j < denklemlersayac + 1; j++)//amac fonksiyonumuzdaki r degerlerinin oldugu sutunlari sifirliyoruz
                {
                    if (j == i)
                        continue;
                    if (simp[j, yerbul] != 0)
                    {
                        double katsayi = -(simp[j, yerbul]);
                        for (int a = 0; a < degiskensayac + toplamsayi; a++)
                        {
                            simp[j, a] = (katsayi * simp[i, a]) + simp[j, a];
                        }
                        if (j == degiskensayac)
                            geciciamacfonkdeger += denklemdeger[i] * katsayi;
                        else denklemdeger[j] += denklemdeger[i] * katsayi;
                    }
                }





            }

            StreamWriter standarthal1 = File.AppendText(@"C:\Users\vaio\Desktop\standarthal.txt");// her adimi yazdiran fonksiyon
            standarthal1.WriteLine();
            for (int i = 0; i < satir.Length; i++)//simpleksin ilk adiminin sonucunu yaziyoruz
            {
                for (int j = 0; j < degiskensayac + toplamsayi; j++)
                {
                    standarthal1.Write(simp[i, j] + "\t\t");

                }
                if (i < denklemlersayac)//denklemlerin degerlerini ve amac fonksiyonunun degerini yazdiriyoruz
                    standarthal1.WriteLine(denklemdeger[i]);
                else standarthal1.WriteLine(geciciamacfonkdeger);
            }
            standarthal1.WriteLine("----------------------------------------------");
            for (int i = 0; i < amacfonkdeger.Length; i++)//amac fonksiyonun degerini yazdiriyoruz 
                amacfonkdeger[i] = -1 * amacfonkdeger[i];

            int kontrol2 = 1;
            double[] yeniamacfonk = new double[degiskensayac + toplamsayi];

            for (int i = 0; i < yeniamacfonk.Length; i++)
            {
                if (i < amacfonkdeger.Length - 1)
                    yeniamacfonk[i] = amacfonkdeger[i];
                else
                    yeniamacfonk[i] = 0;
            }
            //////////////////////////////////////////////////////////////////////////


            //birinci simpleks
            do
            {
                int giren = 0, cikan = 0;
                double girendeger = 0;

                for (int i = 0; i < degiskensayac + toplamsayi; i++)//girecek olan degiskeni seciyoruz
                {
                    if (simp[satir.Length - 1, i] < 0)
                    {
                        if (simp[satir.Length - 1, i] < girendeger)
                        {
                            girendeger = simp[satir.Length - 1, i];
                            giren = i;
                        }

                    }
                }

                double min = double.MaxValue;
                double[] yedekdenklemdeger = new double[denklemlersayac];

                for (int i = 0; i < denklemlersayac; i++)//cikmasi gereken elemani seciyoruz
                {
                    if (simp[i, giren] != 0)
                    {
                        yedekdenklemdeger[i] = denklemdeger[i] / simp[i, giren];
                        if ((yedekdenklemdeger[i] < min) && yedekdenklemdeger[i] > 0)
                        {
                            min = yedekdenklemdeger[i];
                            cikan = i;
                        }
                    }

                }


                hangisi[cikan] = giren + 1;//giren elemani cikan elemanin yerine yaziyoruz
                yerbul = 0;

                standarthal1.WriteLine(giren + "giren sayi  " + simp[denklemlersayac, giren] + cikan + "cikan sayi    ");

                if (simp[denklemlersayac, giren] >= 0)//simpleksi bitirme koşulu
                    break;





                for (int i = 0; i < denklemlersayac; i++)//giren eleman islemleri
                {

                    yerbul = hangisi[i] - 1;
                    double katsayi = (simp[i, yerbul]);
                    for (int j = 0; j < degiskensayac + toplamsayi; j++)
                    {

                        simp[i, j] = simp[i, j] / katsayi;//satiri kendisine boluyoruz(1 yapmak icin)(kendisi = giren yeni degisken )

                    }
                    denklemdeger[i] = denklemdeger[i] / katsayi;//denklemin degerini de kendisine boluyoruz
                }
                for (int i = 0; i < denklemlersayac; i++)//sutundaki giren degiskenin oldugu yerleri sifir yapiyoruz
                {

                    yerbul = hangisi[i] - 1;

                    for (int j = 0; j < denklemlersayac + 1; j++)
                    {
                        if (j == i)
                            continue;
                        if (simp[j, yerbul] != 0)
                        {
                            double katsayi = -(simp[j, yerbul]);
                            for (int a = 0; a < degiskensayac + toplamsayi; a++)
                            {
                                simp[j, a] = (katsayi * simp[i, a]) + simp[j, a];
                            }
                            if (j == degiskensayac)
                                geciciamacfonkdeger += (denklemdeger[i] * katsayi);
                            else denklemdeger[j] += (denklemdeger[i] * katsayi);
                        }
                    }





                }
                for (int i = 0; i < satir.Length; i++)//yaptigimiz islemleri txt dosyasina kaydediyoruz
                {
                    for (int j = 0; j < degiskensayac + toplamsayi; j++)
                    {
                        standarthal1.Write(simp[i, j] + "\t\t");

                    }
                    if (i < denklemlersayac)
                        standarthal1.WriteLine(denklemdeger[i]);
                    else standarthal1.WriteLine(geciciamacfonkdeger);
                }
                standarthal1.WriteLine("----------------------------------------------");

            } while (kontrol2 != -1);

            double[,] simp2 = new double[denklemlersayac + 1, degiskensayac + toplamsayi];
            for (int i = 0; i < degiskensayac + toplamsayi; i++)//ikinci asama icin orijinal amac fonksiyonumuzu kullaniyoruz
            {
                simp[denklemlersayac, i] = yedekamacfonkdeger[i];
            }
            for (int i = 0; i < denklemlersayac + 1; i++)//r'lerimizin hepsi cikti, bunlari tekrardan sokmamak icin tum degerlerini sifir yapiyoruz
            {
                for (int j = 0; j < degiskensayac + toplamsayi; j++)
                {
                    if (j > degiskensayac - 1 && j < degiskensayac + rsayac)
                    {

                        simp[i, j] = 0;
                        simp2[i, j] = simp[i, j];//ikinci asamada simp2 yi kullanacagiz

                    }
                    else simp2[i, j] = simp[i, j];
                    //  standarthal1.Write(simp[i, j] + "\t\t");
                }
            }

            // toplamsayi = toplamsayi - rsayac;
            geciciamacfonkdeger = 0;//amac fonksiyonumuzu degistirdigimiz icin degerini de sifirliyoruz
            for (int i = 0; i < denklemlersayac; i++)//burada standart hale getiriyoruz ( secili olan satirlarin sutunlarini sifir yapiyoruz )
            {
                yerbul = hangisi[i] - 1;

                for (int j = 0; j < denklemlersayac + 1; j++)
                {
                    if (j == i)
                        continue;
                    if (simp2[j, yerbul] != 0)
                    {
                        double katsayi = -(simp2[j, yerbul]);
                        for (int a = 0; a < degiskensayac + toplamsayi; a++)
                        {
                            simp2[j, a] = (katsayi * simp2[i, a]) + simp2[j, a];
                        }
                        if (j == degiskensayac)
                            geciciamacfonkdeger += (denklemdeger[i] * katsayi);
                        else denklemdeger[j] += (denklemdeger[i] * katsayi);
                    }
                }

            }

            for (int i = 0; i < satir.Length; i++)//iterasyonu txt ye yazdiriyoruz
            {
                for (int j = 0; j < degiskensayac + toplamsayi; j++)
                {
                    standarthal1.Write(simp2[i, j] + "\t\t");

                }
                if (i < denklemlersayac)
                    standarthal1.WriteLine(denklemdeger[i]);
                else standarthal1.WriteLine(geciciamacfonkdeger);
            }
            standarthal1.WriteLine("----------------------------------------------");
            do//ikinci simpleks basliyor...(1. simpleksin aynisi)
            {
                int giren = 0, cikan = 0;
                double girendeger = 0;

                if (lines[1] == " MIN")//minimum fonksiyonu giren ve cikan degisken belirlemesi 
                {
                    for (int i = 0; i < degiskensayac + toplamsayi; i++)
                    {
                        if (simp2[satir.Length - 1, i] < 0)
                        {
                            if (simp2[satir.Length - 1, i] < girendeger)
                            {
                                girendeger = simp2[satir.Length - 1, i];
                                giren = i;
                            }

                        }
                    }

                    double min = double.MaxValue;
                    double[] yedekdenklemdeger = new double[denklemlersayac];

                    for (int i = 0; i < denklemlersayac; i++)
                    {
                        if (simp2[i, giren] != 0)
                        {
                            yedekdenklemdeger[i] = denklemdeger[i] / simp2[i, giren];
                            if ((yedekdenklemdeger[i] < min) && yedekdenklemdeger[i] > 0)
                            {
                                min = yedekdenklemdeger[i];
                                cikan = i;
                            }
                        }

                    }
                }
                if (lines[1] == " MAX")//maksimum fonksiyonu giren ve cikan degisken belirlemesi
                {
                    for (int i = 0; i < degiskensayac + toplamsayi; i++)
                    {
                        if (simp2[satir.Length - 1, i] > 0)
                        {
                            if (simp2[satir.Length - 1, i] > girendeger)
                            {
                                girendeger = simp2[satir.Length - 1, i];
                                giren = i;
                            }

                        }
                    }

                    double min = double.MaxValue;
                    double[] yedekdenklemdeger = new double[denklemlersayac];

                    for (int i = 0; i < denklemlersayac; i++)
                    {
                        if (simp2[i, giren] != 0)
                        {
                            yedekdenklemdeger[i] = denklemdeger[i] / simp2[i, giren];
                            if ((yedekdenklemdeger[i] < min) && yedekdenklemdeger[i] > 0)
                            {
                                min = yedekdenklemdeger[i];
                                cikan = i;
                            }
                        }

                    }
                }
                hangisi[cikan] = giren + 1;
                yerbul = 0;

                standarthal1.WriteLine(giren + "giren sayi   " + simp2[denklemlersayac, giren] + cikan + "cikan sayi    ");

                if (lines[1] == " MAX")//maksimum fonksiyonu bitirme kosulu
                    if (simp2[denklemlersayac, giren] <= 0)
                        break;
                if (lines[1] == " MIN")//minimum fonksiyonu bitirme kosulu
                    if (simp2[denklemlersayac, giren] >= 0)
                        break;




                for (int i = 0; i < denklemlersayac; i++)
                {

                    yerbul = hangisi[i] - 1;
                    double katsayi = (simp2[i, yerbul]);
                    for (int j = 0; j < degiskensayac + toplamsayi; j++)
                    {

                        simp2[i, j] = simp2[i, j] / katsayi;

                    }
                    denklemdeger[i] = denklemdeger[i] / katsayi;
                }
                for (int i = 0; i < denklemlersayac; i++)
                {

                    yerbul = hangisi[i] - 1;

                    for (int j = 0; j < denklemlersayac + 1; j++)
                    {
                        if (j == i)
                            continue;
                        if (simp2[j, yerbul] != 0)
                        {
                            double katsayi = -(simp2[j, yerbul]);
                            for (int a = 0; a < degiskensayac + toplamsayi; a++)
                            {
                                simp2[j, a] = (katsayi * simp2[i, a]) + simp2[j, a];
                            }
                            if (j == degiskensayac)
                                geciciamacfonkdeger += (denklemdeger[i] * katsayi);
                            else denklemdeger[j] += (denklemdeger[i] * katsayi);
                        }
                    }





                }
                for (int i = 0; i < satir.Length; i++)
                {
                    for (int j = 0; j < degiskensayac + toplamsayi; j++)
                    {
                        standarthal1.Write(simp2[i, j] + "\t\t");

                    }
                    if (i < denklemlersayac)
                        standarthal1.WriteLine(denklemdeger[i]);
                    else standarthal1.WriteLine(geciciamacfonkdeger);
                }
                standarthal1.WriteLine("----------------------------------------------");


            } while (kontrol2 != -1);//ikinci asama da bitti
            for (int i = 0; i < satir.Length; i++)//txt dosyasına yazdırıyoruz
            {
                for (int j = 0; j < degiskensayac + toplamsayi; j++)
                {
                    standarthal1.Write(simp2[i, j] + "\t\t");

                }
                if (i < denklemlersayac)
                    standarthal1.WriteLine(denklemdeger[i]);
                else standarthal1.WriteLine(geciciamacfonkdeger);
            }
            for (int i = 0; i < denklemlersayac; i++)
            {
                standarthal1.Write("secilen {0} değişken = ", i);
                if (hangisi[i] - 1 < degiskensayac)
                    standarthal1.Write("X{0}", hangisi[i]);
                else
                    standarthal1.Write("S{0}", (hangisi[i] - degiskensayac - rsayac));
                standarthal1.WriteLine();
            }

            standarthal1.WriteLine("sonuç = " + (-geciciamacfonkdeger));

            for (int i = 0; i < denklemlersayac; i++)
            {
                Console.Write("secilen {0} değişken = ", i);
                if (hangisi[i] - 1 < degiskensayac)
                    Console.Write("X{0}", hangisi[i]);
                else
                    Console.Write("S{0}", (hangisi[i] - degiskensayac - rsayac));
                Console.WriteLine();
            }



            Console.WriteLine("sonuç = " + (-geciciamacfonkdeger));

            standarthal1.Close();




            Console.ReadKey();


        }
    }
}
