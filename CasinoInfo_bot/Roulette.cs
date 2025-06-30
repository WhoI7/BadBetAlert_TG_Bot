using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CasinoInfo_bot
{
    internal class Roulette
    {
        string conect = Security.MySQL_Connection;
        private ITelegramBotClient Bot { get; set; }
        private Telegram.Bot.Types.Update update { get; set; }
        public int Coefficient { get; set; }
        public int PriceBet { get; set; }
        public string YourBets { get; set; }

        

        private List<List<InlineKeyboardButton>> Coeficient1to1 = new List<List<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("Виберіть на що ставите:", "-"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("🔴", "🔴"),
                InlineKeyboardButton.WithCallbackData("⚫️", "⚫️")
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("Even", "Even"),
                InlineKeyboardButton.WithCallbackData("Odd", "Odd")
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("1-18", "1-18"),
                InlineKeyboardButton.WithCallbackData("19-36", "19-36")
            },
            new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("🗑 Очистити ставки", "Очистити ставки") },
            new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("📃 Назад", "📃 Назад") }
        };

        private List<List<InlineKeyboardButton>> Coeficient2to1 = new List<List<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("Виберіть дюжену або рядок:", "-"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("1-12", "1-12"),
                InlineKeyboardButton.WithCallbackData("13-24", "13-24"),
                InlineKeyboardButton.WithCallbackData("25-36", "25-36")
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("3,6,...,36", "3,6,...,36"),
                InlineKeyboardButton.WithCallbackData("2,5,...,35", "2,5,...,35"),
                InlineKeyboardButton.WithCallbackData("1,4,...,34", "1,4,...,34")
            },
            new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("🗑 Очистити ставки", "Очистити ставки") },
            new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("📃 Назад", "📃 Назад") }
        };
        private List<List<InlineKeyboardButton>> Coeficient5to1 = new List<List<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("Ставка на 6 чисел:", "-"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("1-6", "1-6"),
                InlineKeyboardButton.WithCallbackData("4-9", "4-9"),
                InlineKeyboardButton.WithCallbackData("7-12", "7-12"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("10-15", "10-15"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("13-18", "13-18"),
                InlineKeyboardButton.WithCallbackData("16-21", "16-21"),
                InlineKeyboardButton.WithCallbackData("19-24", "19-24"),
                
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("22-27", "22-27"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("25-30", "25-30"),
                InlineKeyboardButton.WithCallbackData("28-33", "28-33"),
                InlineKeyboardButton.WithCallbackData("31-36", "31-36"),
            },
            new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("🗑 Очистити ставки", "Очистити ставки") },
            new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("📃 Назад", "📃 Назад") }
        };
        private List<List<InlineKeyboardButton>> Coeficient11to1 = new List<List<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("Ставка на 3 числа:", "-"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("1-3", "1-3"),
                InlineKeyboardButton.WithCallbackData("4-6", "4-6"),
                InlineKeyboardButton.WithCallbackData("7-9", "7-9"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("10-12", "10-12"),
                InlineKeyboardButton.WithCallbackData("13-15", "13-15"),
                InlineKeyboardButton.WithCallbackData("16-18", "16-18"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("19-21", "19-21"),
                InlineKeyboardButton.WithCallbackData("22-24", "22-24"),
                InlineKeyboardButton.WithCallbackData("25-27", "25-27"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("28-30", "28-30"),
                InlineKeyboardButton.WithCallbackData("31-33", "31-33"),
                InlineKeyboardButton.WithCallbackData("34-36", "34-36"),
            },
            new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("🗑 Очистити ставки", "Очистити ставки") },
            new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("📃 Назад", "📃 Назад") }
        };

        private List<List<InlineKeyboardButton>> Coeficient35to1 = new List<List<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("Ставка на число:", "-"),
            },
            new List<InlineKeyboardButton> 
            { 
                InlineKeyboardButton.WithCallbackData("0🟢", "0🟢") 
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("1🔴", "1🔴"),
                InlineKeyboardButton.WithCallbackData("2⚫️", "2⚫️"),
                InlineKeyboardButton.WithCallbackData("3🔴", "3🔴")
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("4⚫️", "4⚫️"),
                InlineKeyboardButton.WithCallbackData("5🔴", "5🔴"),
                InlineKeyboardButton.WithCallbackData("6⚫️", "6⚫️")
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("7🔴", "7🔴"),
                InlineKeyboardButton.WithCallbackData("8⚫️", "8⚫️"),
                InlineKeyboardButton.WithCallbackData("9🔴", "9🔴"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("10⚫️", "10⚫️"),
                InlineKeyboardButton.WithCallbackData("11⚫️", "11⚫️"),
                InlineKeyboardButton.WithCallbackData("12🔴", "12🔴"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("13⚫️", "13⚫️"),
                InlineKeyboardButton.WithCallbackData("14🔴", "14🔴"),
                InlineKeyboardButton.WithCallbackData("15⚫️", "15⚫️"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("16🔴", "16🔴"),
                InlineKeyboardButton.WithCallbackData("17⚫️", "17⚫️"),
                InlineKeyboardButton.WithCallbackData("18🔴", "18🔴"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("19🔴", "19🔴"),
                InlineKeyboardButton.WithCallbackData("20⚫️", "20⚫️"),
                InlineKeyboardButton.WithCallbackData("21🔴", "21🔴"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("22⚫️", "22⚫️"),
                InlineKeyboardButton.WithCallbackData("23🔴", "23🔴"),
                InlineKeyboardButton.WithCallbackData("24⚫️", "24⚫️"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("25🔴", "25🔴"),
                InlineKeyboardButton.WithCallbackData("26⚫️", "26⚫️"),
                InlineKeyboardButton.WithCallbackData("27🔴", "27🔴"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("28⚫️", "28⚫️"),
                InlineKeyboardButton.WithCallbackData("29⚫️", "29⚫️"),
                InlineKeyboardButton.WithCallbackData("30🔴", "30🔴"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("31⚫️", "31⚫️"),
                InlineKeyboardButton.WithCallbackData("32🔴", "32🔴"),
                InlineKeyboardButton.WithCallbackData("33⚫️", "33⚫️"),
            },
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("34🔴", "34🔴"),
                InlineKeyboardButton.WithCallbackData("35⚫️", "35⚫️"),
                InlineKeyboardButton.WithCallbackData("36🔴", "36🔴"),
            },
            new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("🗑 Очистити ставки", "Очистити ставки") },
            new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("📃 Назад", "📃 Назад") }
        };




        
        public Roulette(ITelegramBotClient client, Telegram.Bot.Types.Update update)
        {
            Bot = client;
            this.update = update;
        }
        public async Task SelectAsync()
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();
                
                string comanda = $"SELECT * FROM roulette WHERE ID = @ID;";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;

                    using (var read = await zapros.ExecuteReaderAsync())
                    {
                        read.Read();

                        this.Coefficient = Convert.ToInt32(read["coefficient"]);
                        this.YourBets = Convert.ToString(read["yourbets"]);
                        this.PriceBet = Convert.ToInt32(read["pricebet"]);
                    }

                }
            }
        }
        public async Task UpdateRouletAsync()
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                string comanda = $"UPDATE roulette SET yourbets = @yourbets, coefficient = @coefficient, pricebet = @pricebet  WHERE ID = @ID";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@coefficient", MySqlDbType.UInt32).Value = this.Coefficient;
                    zapros.Parameters.Add("@pricebet", MySqlDbType.UInt32).Value = this.PriceBet;
                    zapros.Parameters.Add("@yourbets", MySqlDbType.String).Value = this.YourBets;


                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                    await zapros.ExecuteNonQueryAsync();
                }

            }
        }
        internal async Task SendBetAsync()
        {
            await SelectAsync();
            using (FileStream stream = System.IO.File.OpenRead(@"roulettetable.png"))
            {
                await Bot.SendPhotoAsync(
                    chatId: update.CallbackQuery.Message.Chat.Id,
                    photo: new InputFileStream(stream, $"1"),
                    caption: $"Ставка: {this.PriceBet}\nКоефіцієнт ставки {this.Coefficient} до 1\nВаші ставки:{this.YourBets}",
                    replyMarkup: GenerateButton());
            }

        }
        
        private InlineKeyboardMarkup GenerateButton()
        {
            var button = new List<List<InlineKeyboardButton>>();
            
            var rad = new List<InlineKeyboardButton>();
            rad = new List<InlineKeyboardButton>();
            rad.Add(InlineKeyboardButton.WithCallbackData("🔄 Крутити рулетку", "🔄 Крутити рулетку"));
            button.Add(rad);

            rad = new List<InlineKeyboardButton>();
            rad.Add(InlineKeyboardButton.WithCallbackData("Ставка:", "-"));
            switch (PriceBet)
            {
                case 1: rad.Add(InlineKeyboardButton.WithCallbackData("⬆️", "betUP")); rad.Add(InlineKeyboardButton.WithCallbackData("-", "-")); break;
                case 5:
                case 25:
                case 50:
                case 100:
                case 500: rad.Add(InlineKeyboardButton.WithCallbackData("⬆️", "betUP")); rad.Add(InlineKeyboardButton.WithCallbackData("⬇️", "betDOWN")); break;
                case 1000: rad.Add(InlineKeyboardButton.WithCallbackData("-", "-")); rad.Add(InlineKeyboardButton.WithCallbackData("⬇️", "betDOWN")); break;
            }
            button.Add(rad);
            rad = new List<InlineKeyboardButton>();

            rad.Add(InlineKeyboardButton.WithCallbackData("Коефіцієнт:", "-"));
            switch (this.Coefficient)
            {
                case 1: rad.Add(InlineKeyboardButton.WithCallbackData("⬆️", "coefUP")); rad.Add(InlineKeyboardButton.WithCallbackData("-", "-")); break;
                case 2:
                case 5:
                case 11: rad.Add(InlineKeyboardButton.WithCallbackData("⬆️", "coefUP")); rad.Add(InlineKeyboardButton.WithCallbackData("⬇️", "coefDOWN")); break;
                case 35: rad.Add(InlineKeyboardButton.WithCallbackData("-", "-")); rad.Add(InlineKeyboardButton.WithCallbackData("⬇️", "coefDOWN")); break;
            }
            button.Add(rad);

           

            switch (this.Coefficient)
            {
                case 1: button.AddRange(Coeficient1to1); break;
                case 2: button.AddRange(Coeficient2to1); break;
                case 5: button.AddRange(Coeficient5to1); break;
                case 11: button.AddRange(Coeficient11to1); break;
                case 35: button.AddRange(Coeficient35to1); break;
            }

            var keyboard = new InlineKeyboardMarkup(button);
            return keyboard;
        }



        internal async Task RouletLogic()
        {
            var words = new List<string>
{
    "0🟢", "1🔴", "2⚫️", "3🔴",
    "4⚫️", "5🔴", "6⚫️", "7🔴",
    "8⚫️", "9🔴", "10⚫️", "11⚫️",
    "12🔴", "13⚫️", "14🔴", "15⚫️",
    "16🔴", "17⚫️", "18🔴", "19🔴",
    "20⚫️", "21🔴", "22⚫️", "23🔴",
    "24⚫️", "25🔴", "26⚫️", "27🔴",
    "28⚫️", "29⚫️", "30🔴", "31⚫️",
    "32🔴", "33⚫️", "34🔴", "35⚫️",
    "36🔴",
    "🔴", "⚫️",
    "Even", "Odd",
    "1-18", "19-36",
    "1-12", "13-24", "25-36",
    "3,6,...,36", "2,5,...,35", "1,4,...,34",
    "1-6", "4-9", "7-12",
    "10-15",
    "13-18", "16-21", "19-24",
    "22-27",
    "25-30", "28-33", "31-36",
    "1-3", "4-6", "7-9",
    "10-12", "13-15", "16-18",
    "19-21", "22-24", "25-27",
    "28-30", "31-33", "34-36"
};

            string perevirka = "- 🟥⬛ Roulette";
            try
            {
                if (!perevirka.Contains(update.CallbackQuery.Data))
                {
                    await SelectAsync();

                    if (update.CallbackQuery.Data == "🔄 Крутити рулетку")
                    {
                        if (YourBets.Length != 0)
                        {
                            string[] bets = YourBets.Split('|');
                            Random random = new Random();
                            int number = random.Next(0, 37);

                            using (FileStream stream = System.IO.File.OpenRead($@"Roulette\{number}.png"))
                            {
                                await Bot.SendPhotoAsync(
                                    chatId: update.CallbackQuery.Message.Chat.Id,
                                    photo: new InputFileStream(stream, $"roulette"),
                                    caption: $"🎯 Випало число: {number}"
                                    );
                            }
                            await CheckSector(bets[0], number);
                            if (this.YourBets.Count(c => c == 'x') >= 4) await CheckSector(bets[1], number);
                            if (this.YourBets.Count(c => c == 'x') >= 6) await CheckSector(bets[2], number);
                            if (this.YourBets.Count(c => c == 'x') >= 8) await CheckSector(bets[3], number);
                            if (this.YourBets.Count(c => c == 'x') == 10) await CheckSector(bets[4], number);

                            Bet bet = new Bet();
                            await bet.ResetBetAsync(update.CallbackQuery.Message.Chat.Id);
                            this.YourBets = "";
                            await this.UpdateRouletAsync();

                            await Bot.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id,update.CallbackQuery.Message.MessageId);

                            await SendBetAsync();
                            
                        }
                        else await Bot.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, "Потрібно зробити ставки!)");
                    }
                    else if (update.CallbackQuery.Data == "Очистити ставки")
                    {
                        Bet bet = new Bet();
                        await bet.clearBet(update.CallbackQuery.Message.Chat.Id);

                        this.YourBets = "";
                    }
                    else if (update.CallbackQuery.Data == "betUP")
                    {
                        switch (this.PriceBet)
                        {
                            case 1: this.PriceBet = 5; break;
                            case 5: this.PriceBet = 25; break;
                            case 25: this.PriceBet = 50; break;
                            case 50: this.PriceBet = 100; break;
                            case 100: this.PriceBet = 500; break;
                            case 500: this.PriceBet = 1000; break;
                        }
                    }
                    else if (update.CallbackQuery.Data == "betDOWN")
                    {
                        switch (this.PriceBet)
                        {
                            case 5: this.PriceBet = 1; break;
                            case 25: this.PriceBet = 5; break;
                            case 50: this.PriceBet = 25; break;
                            case 100: this.PriceBet = 50; break;
                            case 500: this.PriceBet = 100; break;
                            case 1000: this.PriceBet = 500; break;
                        }
                    }
                    else if (update.CallbackQuery.Data == "coefUP")
                    {
                        switch (this.Coefficient)
                        {
                            case 1: this.Coefficient = 2; break;
                            case 2: this.Coefficient = 5; break;
                            case 5: this.Coefficient = 11; break;
                            case 11: this.Coefficient = 35; break;
                        }
                    }
                    else if (update.CallbackQuery.Data == "coefDOWN")
                    {
                        switch (this.Coefficient)
                        {
                            case 2: this.Coefficient = 1; break;
                            case 5: this.Coefficient = 2; break;
                            case 11: this.Coefficient = 5; break;
                            case 35: this.Coefficient = 11; break;
                        }
                    }
                    else if(words.Contains(update.CallbackQuery.Data))
                    {
                        if (this.YourBets.Count(c => c == 'x') < 10)
                        {
                            if (((this.YourBets.Length - this.YourBets.Replace(update.CallbackQuery.Data, "").Length) / update.CallbackQuery.Data.Length) < 2)
                            {
                                using (var connection = new MySqlConnection(conect))
                                {
                                    connection.Open();
                                    Bet Bet = new Bet();
                                    string comanda = $"SELECT `1` , `5` , `25` , `50` , `100` , `500` , `1000`  FROM acaunt WHERE ID = @ID;";
                                    using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                                    {
                                        zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;

                                        using (var read = await zapros.ExecuteReaderAsync())
                                        {
                                            read.Read();
                                            if ((int)read[$"{this.PriceBet}"] != 0)
                                            {
                                                read.Close();
                                                comanda = $"UPDATE acaunt SET `{this.PriceBet}` = `{this.PriceBet}` - 1  WHERE ID = @ID";
                                                using (MySqlCommand zap = new MySqlCommand(comanda, connection))
                                                {
                                                    zap.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                                                    await zap.ExecuteNonQueryAsync();
                                                }

                                                var comand = $"UPDATE Bet SET bet = bet + {this.PriceBet}, `{this.PriceBet}` = `{this.PriceBet}` + 1  WHERE ID = @ID";

                                                using (MySqlCommand za = new MySqlCommand(comand, connection))
                                                {
                                                    za.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                                                    await za.ExecuteNonQueryAsync();
                                                }


                                                this.YourBets += update.CallbackQuery.Data + "x" + this.PriceBet + "x" + this.Coefficient;
                                                if (this.YourBets.Count(c => c == 'x') < 10) this.YourBets += " | ";
                                            }
                                            else await Bot.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"❗️У вас закінчилися фішки номіналом {this.PriceBet}❗️");
                                        }
                                    }
                                }
                            }
                            else await Bot.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, "❗️На одну і ту саму позицію можна ставити тільки 2 рази❗️");
                        }
                        else await Bot.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, "❗️Ви зробили максимальну кількість ставок❗️");
                    }
                    else
                    {
                        Acaunt acaunt = new Acaunt();
                        await acaunt.SelectStageAsync(update.CallbackQuery.Message.Chat.Id);
                        await Bot.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"Ви в \"{acaunt.Stage}\" функція \"{update.CallbackQuery.Data}\" недоступна");
                    }

                    if (update.CallbackQuery.Data != "🔄 Крутити рулетку")
                    {
                        await UpdateRouletAsync();

                        await Bot.EditMessageCaptionAsync(
                            chatId: update.CallbackQuery.Message.Chat.Id,
                            messageId: update.CallbackQuery.Message.MessageId,
                            caption: $"Ставка: {this.PriceBet}\nКоефіцієнт ставки {this.Coefficient} до 1\nВаші ставки:{this.YourBets}",
                            replyMarkup: GenerateButton()
                            );
                    }
                }

            }
            catch (ApiRequestException ex) when (ex.Message.Contains("message is not modified"))
            {
                
            }
        }

        private async Task CheckSector(string v, int rand)
        {
            string[] pole_stavka_coef = v.Split('x');
            pole_stavka_coef[0] = pole_stavka_coef[0].Trim();
            pole_stavka_coef[1] = pole_stavka_coef[1].Trim();
            pole_stavka_coef[2] = pole_stavka_coef[2].Trim();
            if (pole_stavka_coef[2] == "1")
            {
                int[] redSectors = { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };
                int[] blackSectors = { 2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35 }; 
                


                //ставки з коефіцієнтом 1
                if (pole_stavka_coef[0] == "🔴" && redSectors.Contains(rand))
                {
                    //червоний сектор
                    await WinRoulette(pole_stavka_coef);
                   
                }
                else if (pole_stavka_coef[0] == "⚫️" && blackSectors.Contains(rand))
                {
                    //чорний сектор
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "Even" && rand % 2 == 0)
                {
                    //сектор парних чисел
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "Odd" && rand % 2 == 1)
                {
                    //сектор не парних чисел
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "1-18" && rand >= 1 && rand <= 18)
                {
                    //діапазон чисел 1-18
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "19-36" && rand >= 19 && rand <= 36)
                {
                    //діапазон чисел 19-36
                    await WinRoulette(pole_stavka_coef);
                }
                else await LoseRoulette(pole_stavka_coef);

            }
            else if (pole_stavka_coef[2] == "2")
            {
                int[] Column3 = { 3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36 };
                int[] Column2 = { 2, 5, 8, 11, 14, 17, 20, 23, 26, 29, 32, 35 };
                int[] Column1 = { 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34 };
                //ставки з коефіцієнтом 2
                if (pole_stavka_coef[0] == "1-12" && rand >= 1 && rand <= 12)
                {
                    //діапазон чисел 1-12
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "13-24" && rand >= 13 && rand <= 24)
                {
                    //діапазон чисел 13-24
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "25-36" && rand >= 25 && rand <= 36)
                {
                    //діапазон чисел 25-36
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "3,6,...,36" && Column3.Contains(rand))
                {
                    //ряд чисел "3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36"
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "2,5,...,35" && Column2.Contains(rand))
                {
                    //ряд чисел "2, 5, 8, 11, 14, 17, 20, 23, 26, 29, 32, 35"
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "1,4,...,34" && Column1.Contains(rand))
                {
                    //ряд чисел "1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34"
                    await WinRoulette(pole_stavka_coef);
                }
                else await LoseRoulette(pole_stavka_coef);

            }
            else if (pole_stavka_coef[2] == "5")
            {
                //ставки з коефіцієнтом 5 на 6 чисел
                if (pole_stavka_coef[0] == "1-6" && rand >= 1 && rand <= 6)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "4-9" && rand >= 4 && rand <= 9)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "7-12" && rand >= 7 && rand <= 12)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "10-15" && rand >= 10 && rand <= 15)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "13-18" && rand >= 13 && rand <= 18)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "16-21" && rand >= 16 && rand <= 21)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "19-24" && rand >= 19 && rand <= 24)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "22-27" && rand >= 22 && rand <= 27)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "25-30" && rand >= 25 && rand <= 30)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "28-33" && rand >= 28 && rand <= 33)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "31-36" && rand >= 31 && rand <= 36)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else await LoseRoulette(pole_stavka_coef);
                    
            }
            else if (pole_stavka_coef[2] == "11")
            {
                
                //ставки з коефіцієнтом 11 на 3 числа
                if (pole_stavka_coef[0] == "1-3" && rand >= 1 && rand <= 3)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "4-6" && rand >= 4 && rand <= 6)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "7-9" && rand >= 7 && rand <= 9)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "10-12" && rand >= 10 && rand <= 12)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "13-15" && rand >= 13 && rand <= 15)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "16-18" && rand >= 16 && rand <= 18)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "19-21" && rand >= 19 && rand <= 21)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "22-24" && rand >= 22 && rand <= 24)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "25-27" && rand >= 25 && rand <= 27)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "28-30" && rand >= 28 && rand <= 30)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "31-33" && rand >= 31 && rand <= 33)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else if (pole_stavka_coef[0] == "33-36" && rand >= 33 && rand <= 36)
                {
                    await WinRoulette(pole_stavka_coef);
                }
                else await LoseRoulette(pole_stavka_coef);
                    
                
            }
            else if (pole_stavka_coef[2] == "35")
            {
                //ставки з коефіцієнтом 35
                if ((pole_stavka_coef[0].Substring(0, pole_stavka_coef[0].Length - 2)) == rand.ToString())
                {
                    //ставка на число
                    await WinRoulette(pole_stavka_coef);
                }
                else await LoseRoulette(pole_stavka_coef);     
            }
        }

        private async Task WinRoulette(string[] pole_stavka_coef)
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                var comanda = $"UPDATE acaunt SET `{pole_stavka_coef[1]}` = `{pole_stavka_coef[1]}` + {Convert.ToInt32(pole_stavka_coef[2]) +1}, Win = Win + 1, Hands = Hands + 1 WHERE ID = @ID";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                    await zapros.ExecuteNonQueryAsync();
                }
            }

            await Bot.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"🎲 Ставка на сектор {pole_stavka_coef[0]} — ✅ WIN\nСума виграшу : {Convert.ToInt32(pole_stavka_coef[1]) * (Convert.ToInt32(pole_stavka_coef[2])+1)}");
        }
        private async Task LoseRoulette(string[] pole_stavka_coef)
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                var comanda = $"UPDATE acaunt SET Lose = Lose + 1, Hands = Hands + 1 WHERE ID = @ID";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                    await zapros.ExecuteNonQueryAsync();
                }
            }
            await Bot.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"🎲 Ставка на сектор {pole_stavka_coef[0]} — ❌ LOSS");
        }
    }
}
