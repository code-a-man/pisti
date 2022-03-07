using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Pişti {
	class Program {
		static void Main(string[] args)	{
			Console.OutputEncoding = Encoding.UTF8;
			Oyun oyun = new Oyun();
			oyun.start();
		}
		class Oyun	{
			public List<Kart> deste = new List<Kart>();
			public List<Kart> orta = new List<Kart>();	
			public List<Oyuncu> oyuncular = new List<Oyuncu>();
			public int sonAlan = 0;

			public void start() {
				Console.Write("Ad: ");
				string ad = Console.ReadLine();
				Console.Write("Soyad: ");
				string soyad = Console.ReadLine();
				Console.Write("Doğum günü: ");
				int dg = Convert.ToInt32(Console.ReadLine());

				this.oyuncular.Add(new Oyuncu(ad));
				this.oyuncular.Add(new Oyuncu(soyad));
				
				this.DesteOluştur();
				for (int i = 0; i < dg; i++) { this.deste = this.deste.OrderBy(i => Guid.NewGuid()).ToList(); } 

				KartVer("orta"); // Başlangıç için ortaya 4 kart koy
				KartVer("oyuncu"); // Her 2 oyuncuya 4 kart ver
				
				while (deste.Count > 0 || this.oyuncular[1].el.Count() != 0){
					if (this.oyuncular[1].el.Count() == 0) { KartVer("oyuncu"); };
					Oyna(0);
					Oyna(1);
				}
				// OYUN SONU
				this.oyuncular[this.sonAlan].envanter.AddRange(this.orta); // Ortada kalan kartları son alan kişiye ver
				
				int J = this.oyuncular[0].envanter.FindAll(x => x.değer == "J").Count;
				int A = this.oyuncular[0].envanter.FindAll(x => x.değer == "A").Count;
				int Sinek2 = this.oyuncular[0].envanter.FindAll(x => x.değer == "2" && x.tip == "♣️").Count;
				int Karo10 = this.oyuncular[0].envanter.FindAll(x => x.değer == "10" && x.tip == "♦️").Count;
				int KartFazlası = this.oyuncular[0].envanter.Count > this.oyuncular[1].envanter.Count ? 3 : 0;

				this.oyuncular[0].puan += J + A + (Sinek2 * 2) + (Karo10 * 3) + KartFazlası;
				this.oyuncular[1].puan += (4 - J) + (4 - A) + ((1-Sinek2) * 2) + ((1-Karo10) * 3) + (3 - KartFazlası);
				//Oyuncu 1de olmayan kartları ekle

				Console.WriteLine("");
				Console.WriteLine("========OYUNCU 1========");
				Console.WriteLine(this.oyuncular[0].KartlarıGöster());
				Console.WriteLine($"Puan: {this.oyuncular[0].puan}");
				Console.WriteLine(this.oyuncular[0].GeçmişGöster());
				Console.WriteLine("========================");
				Console.WriteLine("");
				Console.WriteLine("========OYUNCU 2========");
				Console.WriteLine(this.oyuncular[1].KartlarıGöster());
				Console.WriteLine($"Puan: {this.oyuncular[1].puan}");
				Console.WriteLine(this.oyuncular[1].GeçmişGöster());
				Console.WriteLine("========================");
			}
			public void DesteOluştur() {
				string[] tipler = new string[13] { "A", "K", "Q", "J", "10", "9", "8", "7", "6", "5", "4", "3", "2" };
				string[] cinsler = new string[4] { "♥", "♠️", "♦️", "♣️" };
				foreach (string t in tipler) { foreach (string c in cinsler) { this.deste.Add(new Kart(t,c)); } }
			}
			public void KartVer(string o) {
				if (o == "orta") {
					this.orta = this.deste.ToArray().Take(4).ToList();
					this.deste.RemoveRange(0,4);
				} else {
					this.oyuncular[0].el = this.deste.ToArray().Take(4).ToList();
					this.deste.RemoveRange(0,4);
					this.oyuncular[0].GeçmişeEkle();
					this.oyuncular[1].el = this.deste.ToArray().Take(4).ToList();
					this.deste.RemoveRange(0,4);
					this.oyuncular[1].GeçmişeEkle();
				}
			}
			public void Oyna(int i) {
				Oyuncu oyuncu = this.oyuncular[i];
				Console.Clear(); // Konsolu her atıştan önce temizle
				string kartlarGöster = string.Join(" | ", oyuncu.el.Select(x => $"{x.tip}{x.değer}").ToArray());
				Kart ortadakiKart = this.orta.Last();
				Console.WriteLine($"Oyuncu {i+1}\nOrtadaki Kart: {ortadakiKart.tip}{ortadakiKart.değer}\nKalan Kart Sayısı: {this.deste.Count}");
				Console.Write($"Elindeki kartlar: {kartlarGöster}\nKart seç (1-{oyuncu.el.Count}): ");
				try	{
					int index = Convert.ToInt32(Console.ReadLine()) - 1;
					Kart oynananKart = oyuncu.el[index];
					oyuncu.el.RemoveAt(index);
					this.orta.Add(oynananKart);
					if (oynananKart.değer == ortadakiKart.değer || (oynananKart.değer == "J" && this.orta.Count != 1)){
						if (this.orta.Count == 2) {
							oyuncu.puan += 10; // Pişti +10
							if (oynananKart.değer == "J" && ortadakiKart.değer == "J") { oyuncu.puan += 10; } // J J +20
						}
						oyuncu.envanter.AddRange(this.orta);
						this.orta.Clear();
						this.orta.Add(new Kart("",""));
						this.sonAlan = i;
					}
				}
				catch (System.Exception) {
					Oyna(i);
				}
			}
		}
		class Oyuncu {
			public List<Kart> el = new List<Kart>();
			public List<Kart> envanter = new List<Kart>();
			public List<String> geçmiş = new List<String>();
			public int puan = 0;
			public string name;
			public Oyuncu(string name) {
				this.name = name;
			}
			public string KartlarıGöster() {
				this.envanter = this.envanter.Where(n => n.tip.Length > 0).ToList(); // Boş kartları çıkar
				return string.Join(" | ", this.envanter.Select(x => $"{x.tip}{x.değer}").ToArray());
			}
			public void GeçmişeEkle() {
				this.geçmiş.Add(string.Join(" | ", this.el.Select(x => $"{x.tip}{x.değer}").ToArray()));
			}
			public string GeçmişGöster() {
				return string.Join("\n", this.geçmiş.Select(x => $"[ {x} ]").ToArray());
			}

		}

		class Kart	{
			public string değer;
			public string tip;
			public Kart(string değer, string tip) {
				this.değer = değer;
				this.tip = tip;
			}
		}
	}
}