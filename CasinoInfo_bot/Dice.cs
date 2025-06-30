using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CasinoInfo_bot
{
    internal class Dice
    {
        string conect = Security.MySQL_Connection;
        private ITelegramBotClient Bot { get; set; }
        private Telegram.Bot.Types.Update update { get; set; }
        private int pricebet {get; set;}
        private int number { get; set;}
        private string diapazon { get; set;}
        private float coeficient { get; set; }
        private int sum { get; set; }
        public Dice(ITelegramBotClient client, Telegram.Bot.Types.Update update)
        {
            Bot = client;
            this.update = update;
        }
        

        internal async Task sendMainDiceAsync()
        {
            await SelectAsync();
            await Bot.SendTextMessageAsync(
                text:$"------------------------------------------------------" +
                $"\nПараметри кидка:" +
                $"\n🎲Число: {this.number}"+
                $"\n📊Діапазон: {this.diapazon}"+
                $"\n💰Ставка: {this.pricebet}" +
                $"\n🏆Коефіцієнт виіграшу: {this.coeficient}\n" +
                $"-------------------------------------------------------",
                chatId: update.CallbackQuery.Message.Chat.Id,
                replyMarkup: GenerateButton()
                );
        }
        private InlineKeyboardMarkup GenerateButton()
        {
            var button = new List<List<InlineKeyboardButton>>();

            var rad = new List<InlineKeyboardButton>();
            rad.Add(InlineKeyboardButton.WithCallbackData("🎲 Кинути кубики", "🎲 Кинути кубики"));
            button.Add(rad);

            rad = new List<InlineKeyboardButton>();
            rad.Add(InlineKeyboardButton.WithCallbackData("Ставка:", "-"));
            switch (this.pricebet)
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
            rad.Add(InlineKeyboardButton.WithCallbackData("Число:", "-"));

            if (this.diapazon == "Over") {
                if (this.number == 2)
                {
                    rad.Add(InlineKeyboardButton.WithCallbackData("⬆️", "numberUP")); rad.Add(InlineKeyboardButton.WithCallbackData("-", "-"));
                }
                else if (this.number > 2 && this.number < 11)
                {
                    rad.Add(InlineKeyboardButton.WithCallbackData("⬆️", "numberUP")); rad.Add(InlineKeyboardButton.WithCallbackData("⬇️", "numberDOWN"));
                }
                else if (this.number == 11)
                {
                    rad.Add(InlineKeyboardButton.WithCallbackData("-", "-")); rad.Add(InlineKeyboardButton.WithCallbackData("⬇️", "numberDOWN"));
                }
            }
            else if (this.diapazon == "Under")
            {
                if (this.number == 3)
                {
                    rad.Add(InlineKeyboardButton.WithCallbackData("⬆️", "numberUP")); rad.Add(InlineKeyboardButton.WithCallbackData("-", "-"));
                }
                else if (this.number > 3 && this.number < 12)
                {
                    rad.Add(InlineKeyboardButton.WithCallbackData("⬆️", "numberUP")); rad.Add(InlineKeyboardButton.WithCallbackData("⬇️", "numberDOWN"));
                }
                else if (this.number == 12)
                {
                    rad.Add(InlineKeyboardButton.WithCallbackData("-", "-")); rad.Add(InlineKeyboardButton.WithCallbackData("⬇️", "numberDOWN"));
                }
            }

            button.Add(rad);
            rad = new List<InlineKeyboardButton>();
            rad.Add(InlineKeyboardButton.WithCallbackData("Діапазон:", "-"));
            if (this.diapazon == "Over")
            {
                rad.Add(InlineKeyboardButton.WithCallbackData("✅Over", "-")); rad.Add(InlineKeyboardButton.WithCallbackData("Under", "Under"));
            }
            else if(this.diapazon == "Under")
            { 
                rad.Add(InlineKeyboardButton.WithCallbackData("Over", "Over")); rad.Add(InlineKeyboardButton.WithCallbackData("✅Under", "-")); 
            }
            button.Add(rad);
            rad = new List<InlineKeyboardButton>();
            rad.Add(InlineKeyboardButton.WithCallbackData("📃 Назад", "📃 Назад"));
            button.Add(rad);
            var keyboard = new InlineKeyboardMarkup(button);
            return keyboard;
        }

        public async Task SelectAsync()
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                string comanda = $"SELECT * FROM dice WHERE ID = @ID;";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;

                    using (var read = await zapros.ExecuteReaderAsync())
                    {
                        read.Read();

                        this.number = Convert.ToInt32(read["number"]);
                        this.pricebet = Convert.ToInt32(read["pricebet"]);
                        this.diapazon = Convert.ToString(read["diapazon"]);
                        this.coeficient = Convert.ToSingle(read["coeficient"]);
                    }
                }
            }
        }

        internal async Task DiceLogic()
        {
            var callbackQuer = new List<string>
            {
                "🎲 Кинути кубики",
                "betUP",
                "betDOWN",
                "numberDOWN",
                "numberUP",
                "Over",
                "Under",
                "-"
            };
            try
            {
                if (callbackQuer.Contains(update.CallbackQuery.Data))
                {
                    await SelectAsync();

                    if (update.CallbackQuery.Data == "🎲 Кинути кубики")
                    {
                        using (var connection = new MySqlConnection(conect))
                        {
                            await connection.OpenAsync();

                            string comanda = $"SELECT `{this.pricebet}` FROM acaunt WHERE ID = @ID";
                            using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                            {
                                zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;

                                using (var read = await zapros.ExecuteReaderAsync())
                                {
                                    read.Read();
                                    if (Convert.ToInt32(read[$"{this.pricebet}"]) > 0)
                                    {
                                        var dice1 = await Bot.SendDiceAsync(update.CallbackQuery.Message.Chat.Id, emoji: Emoji.Dice);
                                        var dice2 = await Bot.SendDiceAsync(update.CallbackQuery.Message.Chat.Id, emoji: Emoji.Dice);
                                        this.sum = dice1.Dice.Value + dice2.Dice.Value;

                                        if ((this.diapazon == "Over" && this.sum > this.number) || (this.diapazon == "Under" && this.sum < this.number))
                                        {
                                            await Bot.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"🎉Вам пощастило, випало {this.sum} з можливих {this.diapazon} {this.number}!" +
                                                $"\nВиіграш становить {Math.Round(this.pricebet * this.coeficient)}");
                                            await Win();
                                        }
                                        else
                                        {
                                            await Bot.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"😞Ви програли, випало {this.sum} з можливих {this.diapazon} {this.number}!");
                                            await Loss();
                                        }
                                        await Bot.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                                        await sendMainDiceAsync();
                                    }
                                    else
                                    {
                                        await Bot.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"❗️У вас закінчилися фішки номіналом {this.pricebet}❗️");
                                    }

                                }
                            }
                        }


                    }
                    else if (update.CallbackQuery.Data == "betUP")
                    {
                        switch (this.pricebet)
                        {
                            case 1: this.pricebet = 5; break;
                            case 5: this.pricebet = 25; break;
                            case 25: this.pricebet = 50; break;
                            case 50: this.pricebet = 100; break;
                            case 100: this.pricebet = 500; break;
                            case 500: this.pricebet = 1000; break;
                        }
                    }
                    else if (update.CallbackQuery.Data == "betDOWN")
                    {
                        switch (this.pricebet)
                        {
                            case 5: this.pricebet = 1; break;
                            case 25: this.pricebet = 5; break;
                            case 50: this.pricebet = 25; break;
                            case 100: this.pricebet = 50; break;
                            case 500: this.pricebet = 100; break;
                            case 1000: this.pricebet = 500; break;
                        }
                    }
                    else if (update.CallbackQuery.Data == "numberUP")
                    {
                        if (this.diapazon == "Over" && this.number != 11)
                        {
                            this.number++;
                            switch (this.number)
                            {
                                case 3: this.coeficient = 1.07f; break;
                                case 4: this.coeficient = 1.18f; break;
                                case 5: this.coeficient = 1.36f; break;
                                case 6: this.coeficient = 1.68f; break;
                                case 7: this.coeficient = 2.35f; break;
                                case 8: this.coeficient = 3.53f; break;
                                case 9: this.coeficient = 5.88f; break;
                                case 10: this.coeficient = 11.08f; break;
                                case 11: this.coeficient = 35.3f; break;

                            }
                        }
                        else if (this.diapazon == "Under" && this.number != 12)
                        {
                            this.number++;
                            switch (this.number)
                            {
                                case 12: this.coeficient = 1.01f; break;
                                case 11: this.coeficient = 1.07f; break;
                                case 10: this.coeficient = 1.18f; break;
                                case 9: this.coeficient = 1.36f; break;
                                case 8: this.coeficient = 1.68f; break;
                                case 7: this.coeficient = 2.35f; break;
                                case 6: this.coeficient = 3.53f; break;
                                case 5: this.coeficient = 5.88f; break;
                                case 4: this.coeficient = 11.08f; break;
                                case 3: this.coeficient = 35.3f; break;

                            }
                        }

                    }
                    else if (update.CallbackQuery.Data == "numberDOWN")
                    {
                        if (this.diapazon == "Over" && this.number != 2)
                        {
                            this.number--;
                            switch (this.number)
                            {
                                case 2: this.coeficient = 1.01f; break;
                                case 3: this.coeficient = 1.07f; break;
                                case 4: this.coeficient = 1.18f; break;
                                case 5: this.coeficient = 1.36f; break;
                                case 6: this.coeficient = 1.68f; break;
                                case 7: this.coeficient = 2.35f; break;
                                case 8: this.coeficient = 3.53f; break;
                                case 9: this.coeficient = 5.88f; break;
                                case 10: this.coeficient = 11.08f; break;
                                case 11: this.coeficient = 35.3f; break;

                            }
                        }
                        else if (this.diapazon == "Under" && this.number != 3)
                        {
                            this.number--;
                            switch (this.number)
                            {
                                case 11: this.coeficient = 1.07f; break;
                                case 10: this.coeficient = 1.18f; break;
                                case 9: this.coeficient = 1.36f; break;
                                case 8: this.coeficient = 1.68f; break;
                                case 7: this.coeficient = 2.35f; break;
                                case 6: this.coeficient = 3.53f; break;
                                case 5: this.coeficient = 5.88f; break;
                                case 4: this.coeficient = 11.08f; break;
                                case 3: this.coeficient = 35.3f; break;

                            }
                        }
                    }
                    else if (update.CallbackQuery.Data == "Over")
                    {
                        this.diapazon = "Over";
                        if (this.number == 12)
                        {
                            this.number = 11;
                        }
                        switch (this.number)
                        {
                            case 3: this.coeficient = 1.07f; break;
                            case 4: this.coeficient = 1.18f; break;
                            case 5: this.coeficient = 1.36f; break;
                            case 6: this.coeficient = 1.68f; break;
                            case 7: this.coeficient = 2.35f; break;
                            case 8: this.coeficient = 3.53f; break;
                            case 9: this.coeficient = 5.88f; break;
                            case 10: this.coeficient = 11.08f; break;
                            case 11: this.coeficient = 35.3f; break;
                        }
                    }
                    else if (update.CallbackQuery.Data == "Under")
                    {
                        this.diapazon = "Under";
                        if (this.number == 2)
                        {
                            this.number = 3;
                        }
                        switch (this.number)
                        {
                            case 12: this.coeficient = 1.01f; break;
                            case 11: this.coeficient = 1.07f; break;
                            case 10: this.coeficient = 1.18f; break;
                            case 9: this.coeficient = 1.36f; break;
                            case 8: this.coeficient = 1.68f; break;
                            case 7: this.coeficient = 2.35f; break;
                            case 6: this.coeficient = 3.53f; break;
                            case 5: this.coeficient = 5.88f; break;
                            case 4: this.coeficient = 11.08f; break;
                            case 3: this.coeficient = 35.3f; break;
                        }
                    }

                    if (update.CallbackQuery.Data != "🎲 Кинути кубики")
                    {

                        await UpdateDiceAsync();

                        await Bot.EditMessageTextAsync(
                            chatId: update.CallbackQuery.Message.Chat.Id,
                            messageId: update.CallbackQuery.Message.MessageId,
                            text: $"------------------------------------------------------" +
                                $"\nПараметри кидка:" +
                                $"\n🎲Число: {this.number}" +
                                $"\n📊Діапазон: {this.diapazon}" +
                                $"\n💰Ставка: {this.pricebet}" +
                                $"\n🏆Коефіцієнт виіграшу: {this.coeficient}\n" +
                                $"-------------------------------------------------------",
                                replyMarkup: GenerateButton()
                                );
                    }
                }
                else
                {
                    Acaunt acaunt = new Acaunt();
                    await acaunt.SelectStageAsync(update.CallbackQuery.Message.Chat.Id);
                    await Bot.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"Ви в \"{acaunt.Stage}\" функція \"{update.CallbackQuery.Data}\" недоступна");
                }


            }
            catch (ApiRequestException ex) when (ex.Message.Contains("message is not modified"))
            {

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Dice: {update.CallbackQuery.Message.Chat.Id}:: {ex.Message}");
            }
        }

        private async Task Win()
        {
            Bet bet = new Bet();
            bet.bet = Convert.ToInt32(Math.Round(this.pricebet * this.coeficient) - this.pricebet);
            if (bet.bet / 1000 >= 1)
            {
                bet.C1000 = bet.bet / 1000;
                bet.bet = bet.bet - bet.bet / 1000 * 1000;
            }
            if (bet.bet / 500 >= 1)
            {
                bet.C500 = bet.bet / 500;
                bet.bet = bet.bet - bet.bet / 500 * 500;
            }
            if (bet.bet / 100 >= 1)
            {
                bet.C100 = bet.bet / 100;
                bet.bet = bet.bet - bet.bet / 100 * 100;
            }
            if (bet.bet / 50 >= 1)
            {
                bet.C50 = bet.bet / 50;
                bet.bet = bet.bet - bet.bet / 50 * 50;
            }
            if (bet.bet / 25 >= 1)
            {
                bet.C25 = bet.bet / 25;
                bet.bet = bet.bet - bet.bet / 25 * 25;
            }
            if (bet.bet / 5 >= 1)
            {
                bet.C5 = bet.bet / 5;
                bet.bet = bet.bet - bet.bet / 5 * 5;
            }
            if (bet.bet / 1 >= 1)
            {
                bet.C1 = bet.bet / 1;
            }

            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                var comanda = $"UPDATE acaunt SET `1` = `1` + {bet.C1}, `5` = `5` + {bet.C5}, `25` = `25` + {bet.C25}, `50` = `50` + {bet.C50}, `100` = `100` + {bet.C100}, `500` = `500` + {bet.C500}, `1000` = `1000` + {bet.C1000}, Win = Win + 1, Hands = Hands + 1 WHERE ID = @ID";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                    await zapros.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task Loss()
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                var comanda = $"UPDATE acaunt SET `{this.pricebet}` = `{this.pricebet}` - 1 , Lose = Lose + 1, Hands = Hands + 1 WHERE ID = @ID";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                    await zapros.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task UpdateDiceAsync()
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                string comanda = $"UPDATE dice SET pricebet = @pricebet, coeficient = @coeficient, number = @number, diapazon = @diapazon  WHERE ID = @ID";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@coeficient", MySqlDbType.Float).Value = this.coeficient;
                    zapros.Parameters.Add("@number", MySqlDbType.Int32).Value = this.number;
                    zapros.Parameters.Add("@diapazon", MySqlDbType.String).Value = this.diapazon;
                    zapros.Parameters.Add("@pricebet", MySqlDbType.Int32).Value = this.pricebet;

                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                    await zapros.ExecuteNonQueryAsync();
                }

            }
        }
    }
}
