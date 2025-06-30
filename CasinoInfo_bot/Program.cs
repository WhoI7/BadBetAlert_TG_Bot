using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static Mysqlx.Error.Types;
using File = System.IO.File;

namespace CasinoInfo_bot
{

    internal class Program
    {

        private static TelegramBotClient Bot;
        private static string connectionString = Security.MySQL_Connection;

        static async Task Main(string[] args)
        {


            var cts = new CancellationTokenSource();

            Bot = new TelegramBotClient(Security.TG_BOT_API);

            
            Console.WriteLine("Start log:");



            var UPDATE = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
            };


            Bot.StartReceiving(Update, Error, receiverOptions: UPDATE, cancellationToken: cts.Token);



            Console.ReadKey();
            await Console.Out.WriteLineAsync("End...");
            cts.Cancel();
            await Task.Delay(6000);
        }

        private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine("Error occurred: " + exception.Message);
            return Task.CompletedTask;
        }

        private static async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message)
            {
                await MMessage(client, update);
                await Console.Out.WriteLineAsync($"{update.Message.Chat.Id} дiя повідомлення {update.Message.Text}");
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                await MCallbackQuery(client, update);
                await Console.Out.WriteLineAsync($"{update.CallbackQuery.Message.Chat.Id} дiя кнопка {update.CallbackQuery.Data}");
            }
        }
        private static async Task MMessage(ITelegramBotClient client, Update update)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    int Peopleacaunt;


                    var perevirkauser = $"SELECT EXISTS (SELECT 1 FROM acaunt WHERE ID = ?);";
                    using (MySqlCommand zapros = new MySqlCommand(perevirkauser, connection))
                    {
                        zapros.Parameters.Add("?", MySqlDbType.Int64).Value = update.Message.Chat.Id;
                        Peopleacaunt = Convert.ToInt32(await zapros.ExecuteScalarAsync());
                    }

                    if (Peopleacaunt == 0)
                    {
                        await privacy_policy(client, update.Message.Chat.Id);
                    }
                    else if (Peopleacaunt == 1)
                    {
                        Acaunt user = new Acaunt();
                        await user.SelectStageAndNNAsync(update.Message.Chat.Id);
                        if (update.Message.Text == "/profile")
                        {
                            if (user.NN != "")
                            {
                                string perevirkaNN = $@"START TRANSACTION; UPDATE `21game` SET card = '', bankcard = '' , playercard = '' , bankpoint = 0 , playerpoint = 0 WHERE ID = @ID; UPDATE bet SET bet = 0, `1` = 0, `5` = 0, `25` = 0, `50` = 0, `100` = 0, `500` = 0, `1000` = 0 WHERE ID = @ID;UPDATE roulette SET yourbets = '' WHERE ID = @ID; COMMIT;";
                                using (MySqlCommand zapros = new MySqlCommand(perevirkaNN, connection))
                                {
                                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.Message.Chat.Id;
                                    await zapros.ExecuteNonQueryAsync();
                                }


                                await Profil(update.Message.Chat.Id);
                            }
                            else await Bot.SendTextMessageAsync(update.Message.Chat.Id,"Функція стане доступна після реєстрації в демо грі.");
                        }
                        else if (user.Stage == "register" || user.Stage == "✏️ Змінити")
                        {
                            string umova = @"^[a-zA-Z0-9_-]{3,15}$";
                            bool isValid = Regex.IsMatch(update.Message.Text, umova);
                            if (isValid)
                            {
                                int PeopleNN;

                                perevirkauser = $"SELECT EXISTS (SELECT 1 FROM acaunt WHERE BINARY NN = @NN);";
                                using (MySqlCommand zapros = new MySqlCommand(perevirkauser, connection))
                                {
                                    zapros.Parameters.Add("@NN", MySqlDbType.String).Value = update.Message.Text;
                                    PeopleNN = Convert.ToInt32(await zapros.ExecuteScalarAsync());
                                }
                                if (PeopleNN == 0)
                                {
                                    string perevirkaNN = $"UPDATE acaunt SET NN = @NN where ID = @ID";
                                    using (MySqlCommand zapros = new MySqlCommand(perevirkaNN, connection))
                                    {
                                        zapros.Parameters.AddWithValue("@NN", update.Message.Text);
                                        zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.Message.Chat.Id;
                                        await zapros.ExecuteNonQueryAsync();
                                    }

                                    if (user.Stage == "register") await client.SendTextMessageAsync(update.Message.Chat.Id, $"Вітаю {update.Message.Text}, у вас чудовий ігровий нікнейм.\nБажаю гaрної гри та проведення часу)");
                                    await Profil(update.Message.Chat.Id);

                                }
                                else await client.SendTextMessageAsync(update.Message.Chat.Id, "Цей нікнейм зайнятий\nСпробуй щось інше)");
                            }
                            else
                            {
                                await Criteri(update.Message.Chat.Id, "Написане вами ігрове ім'я не підходить по критеріям😢\nСпробуйте знову.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"MMessage: {update.Message.Chat.Id}:: {ex.Message}");
            }
        }
        private static async Task privacy_policy(ITelegramBotClient client, long Id)
        {
            var Button = new InlineKeyboardMarkup(
                                    new[]
                                    {
                                     new[]
                                     {
                                         InlineKeyboardButton.WithUrl("Політика конфіденційності","https://drive.google.com/file/d/1JJCBQv7b91rTVQCdee2nHWKWtrlJiFeS/view?usp=sharing"),
                                     },
                                     new[]
                                     {
                                         InlineKeyboardButton.WithCallbackData("📑✅", "📑✅"),
                                     }
                                    });

            using (FileStream stream = System.IO.File.OpenRead(@"privacy_policy\privacy_policy.jpg"))
            {
                await client.SendPhotoAsync(
                    chatId: Id,
                    photo: new InputFileStream(stream, $"privacy_policy"),
                    caption: $"Перед тим як почати, будь ласка, ознайомтеся з нашою Політикою конфіденційності. \nНатисніть '📑✅' для дозволу обробки ваших даних.",
                    replyMarkup: Button);
            }
        }

        private static async Task MCallbackQuery(ITelegramBotClient client, Update update)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    int Peopleacaunt;
                    

                    var perevirkauser = $"SELECT EXISTS (SELECT 1 FROM acaunt WHERE ID = ?);";
                    using (MySqlCommand zapros = new MySqlCommand(perevirkauser, connection))
                    {
                        zapros.Parameters.Add("?", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                        Peopleacaunt = Convert.ToInt32(await zapros.ExecuteScalarAsync());
                        
                    }


                    if (Peopleacaunt == 0)
                    {
                        if (update.CallbackQuery.Data == "📑✅")
                        {
                            await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                            var createuser = $"INSERT INTO acaunt (ID) VALUES (?)";
                            using (MySqlCommand zapros = new MySqlCommand(createuser, connection))
                            {
                                zapros.Parameters.Add("?", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                                await zapros.ExecuteNonQueryAsync();
                            }

                            var createbet = $"INSERT INTO bet (ID) VALUES (?)";
                            using (MySqlCommand zapros = new MySqlCommand(createbet, connection))
                            {
                                zapros.Parameters.Add("?", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                                await zapros.ExecuteNonQueryAsync();
                            }

                            var create21game = $"INSERT INTO 21game (ID) VALUES (?)";
                            using (MySqlCommand zapros = new MySqlCommand(create21game, connection))
                            {
                                zapros.Parameters.Add("?", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                                await zapros.ExecuteNonQueryAsync();
                            }

                            var createroulette = $"INSERT INTO roulette (ID) VALUES (?)";
                            using (MySqlCommand zapros = new MySqlCommand(createroulette, connection))
                            {
                                zapros.Parameters.Add("?", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                                await zapros.ExecuteNonQueryAsync();
                            }

                            var createdice = $"INSERT INTO dice (ID) VALUES (?)";
                            using (MySqlCommand zapros = new MySqlCommand(createdice, connection))
                            {
                                zapros.Parameters.Add("?", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                                await zapros.ExecuteNonQueryAsync();
                            }
                            await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Що ж, тепер ви можете повноцінно користуватися ботом. Бажаю вам весело та з користю провести час!");
                           
                            await Info.Menu(client, update);
                           
                        }
                        else await privacy_policy(client, update.CallbackQuery.Message.Chat.Id);

                    }
                    else if (Peopleacaunt == 1)
                    {
                        if (update.CallbackQuery.Data == "📑✅")
                        {
                            await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                        }

                        Acaunt user = new Acaunt();
                        await user.SelectStageAsync(update.CallbackQuery.Message.Chat.Id);
                        if (update.CallbackQuery.Data == "Відміна" && user.Stage != "✏️ Змінити") await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                        else if (user.Stage == "Головне меню")
                        {
                            if (update.CallbackQuery.Data == "info_casino")
                            {
                                await Info.SendCasinoInfoAsync(client, update);

                            }
                            else if (update.CallbackQuery.Data == "info_games")
                            {
                                await Info.SendCasinoGameInfoAsync(client, update);
                            }
                            else if (update.CallbackQuery.Data == "safety_tips")
                            {
                                await Info.SendCasinoSafetyInfoAsync(client, update);
                            }
                            else if (update.CallbackQuery.Data == "Profil")
                            {

                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                                await user.SelectNNAsync(update.CallbackQuery.Message.Chat.Id);
                                if (user.NN != "")
                                {
                                    await Profil(update.CallbackQuery.Message.Chat.Id);
                                }
                                else
                                {
                                    await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Ой-ой... Я тут помітив, що у тебе досі немає власного ігрового нікнейму. Зараз ми це виправимо!");
                                    await Criteri(update.CallbackQuery.Message.Chat.Id, "!!УВАГА: нікнейм можна в подальшому змінити!!\nПридумайте собі класний ігровий нікнейм!)");
                                    await user.UpdateStageAsync(update.CallbackQuery.Message.Chat.Id, "register");
                                }
                            }
                            else
                            {
                                await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"Ви в \"{user.Stage}\" функція \"{update.CallbackQuery.Data}\" недоступна");
                            }

                        }
                        else if (user.Stage == "CasinoInfo")
                        {
                            if (update.CallbackQuery.Data == "⬅️")
                            {
                                await Info.MoveLeftCasinoInfo(client, update);
                            }
                            else if (update.CallbackQuery.Data == "➡️")
                            {
                                await Info.MoveRightCasinoInfo(client, update);
                            }
                            else if (update.CallbackQuery.Data == "Головне меню")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                                
                                await Info.Menu(client, update);
                            }
                            else
                            {
                                await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"Ви в \"{user.Stage}\" функція \"{update.CallbackQuery.Data}\" недоступна");
                            }
                        }
                        else if (user.Stage == "CasinoGameInfo")
                        {
                            if (update.CallbackQuery.Data == "⬅️")
                            {
                                
                                await Info.MoveLeftCasinoGameInfo(client, update);
                            }
                            else if (update.CallbackQuery.Data == "➡️")
                            {
                                
                                await Info.MoveRightCasinoGameInfo(client, update);
                            }
                            else if (update.CallbackQuery.Data == "Головне меню")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                                
                                await Info.Menu(client, update);
                            }
                            else
                            {
                                await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"Ви в \"{user.Stage}\" функція \"{update.CallbackQuery.Data}\" недоступна");
                            }
                        }
                        else if (user.Stage == "CasinoSafetyInfo")
                        {
                            if (update.CallbackQuery.Data == "⬅️")
                            {
                                
                                await Info.MoveLeftCasinoSafetyInfo(client, update);
                            }
                            else if (update.CallbackQuery.Data == "➡️")
                            {
                                
                                await Info.MoveRightCasinoSafetyInfo(client, update);
                            }
                            else if (update.CallbackQuery.Data == "Головне меню")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                                
                                await Info.Menu(client, update);
                            }
                            else
                            {
                                await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"Ви в \"{user.Stage}\" функція \"{update.CallbackQuery.Data}\" недоступна");
                            }
                        }
                        else if (user.Stage == "📄 Profil")
                        {
                            if (update.CallbackQuery.Data == "🎲 Ігри")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                await gamelist(update.CallbackQuery.Message.Chat.Id);
                            }
                            else if (update.CallbackQuery.Data == "🏆 Рейтинг")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                var Button = new InlineKeyboardMarkup(
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("📄 Назад до профілю", "📄 Назад до профілю"),
                                    });

                                Acaunt acaunt = new Acaunt();
                                await acaunt.UpdateStageAsync(update.CallbackQuery.Message.Chat.Id, "Top");
                                await acaunt.TopPlayersBalancAsync();
                                await Bot.SendTextMessageAsync(
                                    update.CallbackQuery.Message.Chat.Id,
                                    acaunt.Top,
                                    replyMarkup: Button);
                            }
                            else if (update.CallbackQuery.Data == "⚙️ Налаштування")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                                await user.UpdateStageAsync(update.CallbackQuery.Message.Chat.Id, "⚙️ Налаштування");
                                var Button = new InlineKeyboardMarkup(
                                    new[]
                                    {
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("✏️ Змінити", "✏️ Змінити"),
                                            InlineKeyboardButton.WithCallbackData("❌ Видалити", "❌ Видалити"),
                                        },
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("📄 Назад до профілю", "📄 Назад до профілю"),
                                        }
                                    });

                                await client.SendTextMessageAsync(
                                    chatId: update.CallbackQuery.Message.Chat.Id,
                                    text: "Міні налаштування:\n" +
                                    "✏️ Змінити - дає можливість змінити нік.\n" +
                                    "❌ Видалити - ВИДАЛЕННЯ акаунту!",
                                    replyMarkup: Button);
                            }
                            else if (update.CallbackQuery.Data == "🔄 Обмін фішок")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                                await ChipExchange(update.CallbackQuery.Message.Chat.Id);
                            }
                            else if (update.CallbackQuery.Data == "Головне меню")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                                
                                await Info.Menu(client, update);
                            }
                            else
                            {
                                await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"Ви в \"{user.Stage}\" функція \"{update.CallbackQuery.Data}\" недоступна");
                            }

                        }
                        else if (user.Stage == "🎲 Ігри")
                        {
                            if (update.CallbackQuery.Data == "🂡 Black Jack")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                var Button = new InlineKeyboardMarkup(
                                    new[]
                                    {
                                         InlineKeyboardButton.WithCallbackData("📃 Назад", "📃 Назад"),
                                         InlineKeyboardButton.WithCallbackData("🗑️ Очистити", "🗑️ Очистити"),
                                         InlineKeyboardButton.WithCallbackData("Грати", "Грати"),

                                    }
                                );
                                await user.UpdateStageAsync(update.CallbackQuery.Message.Chat.Id, "🂡 Black Jack");
                                using (FileStream stream = System.IO.File.OpenRead(@"bet.jpg"))
                                {
                                    await client.SendPhotoAsync(
                                        chatId: update.CallbackQuery.Message.Chat.Id,
                                        photo: new InputFileStream(stream, $"ставка"),
                                        caption: $"━━━━━━━━━ {update.CallbackQuery.Data} ━━━━━━━━━\n" +
                                        "Ваша ставка: 0",
                                         replyMarkup: new InlineKeyboardMarkup(((await GenerateButton(update.CallbackQuery.Message.Chat.Id)).InlineKeyboard.Concat(Button.InlineKeyboard).ToArray()))
                                         );
                                }
                            }
                            else if (update.CallbackQuery.Data == "🎰 Classic Slots")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                await user.UpdateStageAsync(update.CallbackQuery.Message.Chat.Id, "🎰 Classic Slots");

                                var Buttonslot = new InlineKeyboardMarkup(
                                    new[]
                                    {
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("🔽 Зменшити ставку", "🔽 Зменшити ставку"),
                                            InlineKeyboardButton.WithCallbackData("🔼 Збільшити ставку", "🔼 Збільшити ставку"),

                                        },
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("🎲 Крутити", "🎲 Крутити"),
                                        },
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("📃 Назад", "📃 Назад"),
                                        }
                                    });

                                await client.SendTextMessageAsync(
                                    chatId: update.CallbackQuery.Message.Chat.Id,
                                    text: "------------------------------------------------------------------\n" +
                                            "|                         | [🍒][🍒][🍒] |                    |\n" +
                                            "|                      → [7️⃣][7️⃣][7️⃣] ←                 |\n" +
                                            "|                         | [🍋][🍋][🍋] |                    |\n" +
                                            "------------------------------------------------------------------\n" +
                                            "Ваша ставка: 1\n" +
                                            "Ваш виіграш: ---------",
                                    replyMarkup: Buttonslot);

                            }
                            else if (update.CallbackQuery.Data == "🟥⬛ Roulette")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                Acaunt acaunt = new Acaunt();
                                await acaunt.UpdateStageAsync(update.CallbackQuery.Message.Chat.Id, "🟥⬛ Roulette");
                                Roulette roulette = new Roulette(client, update);
                                await roulette.SendBetAsync();
                            }
                            else if (update.CallbackQuery.Data == "🎲 Rocket Dice")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                Acaunt acaunt = new Acaunt();
                                await acaunt.UpdateStageAsync(update.CallbackQuery.Message.Chat.Id, "🎲 Rocket Dice");
                                Dice dice = new Dice(client, update);
                                await dice.sendMainDiceAsync();

                            }
                            else if (update.CallbackQuery.Data == "🚧 Скоро")
                            {
                                ////////////////////////////////////////////////
                            }
                            else if (update.CallbackQuery.Data == "📄 Назад до профілю")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                await Profil(update.CallbackQuery.Message.Chat.Id);
                            }
                            else
                            {
                                await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"Ви в \"{user.Stage}\" функція \"{update.CallbackQuery.Data}\" недоступна");
                            }
                        }
                        else if (user.Stage == "Top")
                        {
                            if (update.CallbackQuery.Data == "📄 Назад до профілю")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                await Profil(update.CallbackQuery.Message.Chat.Id);
                            }
                            else
                            {
                                await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"Ви в \"{user.Stage}\" функція \"{update.CallbackQuery.Data}\" недоступна");
                            }
                        }
                        else if (user.Stage == "⚙️ Налаштування")
                        {
                            if (update.CallbackQuery.Data == "✏️ Змінити")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                await user.UpdateStageAsync(update.CallbackQuery.Message.Chat.Id, "✏️ Змінити");

                                var Renameremove = new InlineKeyboardMarkup(
                                    new[]{
                                    InlineKeyboardButton.WithCallbackData("Відміна", "Відміна"),
                                    });

                                await client.SendTextMessageAsync(
                                    chatId: update.CallbackQuery.Message.Chat.Id,
                                    text: "Критерії, допустимі символи:\n" +
                                        "• Латинські літери(A-Z, a-z)\n" +
                                        "• Цифри: 0-9\n" +
                                        "• Підкреслення (_), дефіс (-)\n" +
                                        "• Без відступів(пробілів)\n" +
                                        "• Довжина min(5) max(15) символів\n\n" +
                                        "Введіть ваш новий нік.\n\n" +
                                        "Cкористайтеся кнопкою нище для відміни операції переіменування.",
                                    replyMarkup: Renameremove);
                            }
                            else if (update.CallbackQuery.Data == "❌ Видалити")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                                await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Жаль що ми прощаємося до зустрічі(\n" +
                                    "Ваші дані було видалено");
                                string comanda = $"DELETE FROM acaunt WHERE ID = ?";
                                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                                {
                                    zapros.Parameters.Add("?", MySqlDbType.Double).Value = update.CallbackQuery.Message.Chat.Id;
                                    await zapros.ExecuteNonQueryAsync();
                                }
                                comanda = $"DELETE FROM 21game WHERE ID = ?";
                                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                                {
                                    zapros.Parameters.Add("?", MySqlDbType.Double).Value = update.CallbackQuery.Message.Chat.Id;
                                    await zapros.ExecuteNonQueryAsync();
                                }
                                comanda = $"DELETE FROM bet WHERE ID = ?";
                                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                                {
                                    zapros.Parameters.Add("?", MySqlDbType.Double).Value = update.CallbackQuery.Message.Chat.Id;
                                    await zapros.ExecuteNonQueryAsync();
                                }
                                comanda = $"DELETE FROM roulette WHERE ID = ?";
                                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                                {
                                    zapros.Parameters.Add("?", MySqlDbType.Double).Value = update.CallbackQuery.Message.Chat.Id;
                                    await zapros.ExecuteNonQueryAsync();
                                }
                                comanda = $"DELETE FROM dice WHERE ID = ?";
                                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                                {
                                    zapros.Parameters.Add("?", MySqlDbType.Double).Value = update.CallbackQuery.Message.Chat.Id;
                                    await zapros.ExecuteNonQueryAsync();
                                }
                            }
                            else if (update.CallbackQuery.Data == "📄 Назад до профілю")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                await Profil(update.CallbackQuery.Message.Chat.Id);
                            }
                            else
                            {
                                await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"Ви в \"{user.Stage}\" функція \"{update.CallbackQuery.Data}\" недоступна");
                            }
                        }
                        else if (user.Stage == "✏️ Змінити")
                        {
                            if (update.CallbackQuery.Data == "Відміна")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                await Profil(update.CallbackQuery.Message.Chat.Id);
                            }
                            else
                            {
                                await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"Ви в \"{user.Stage}\" функція \"{update.CallbackQuery.Data}\" недоступна");
                            }
                        }
                        else if (user.Stage == "🔄 Обмін фішок")
                        {
                            string[] fishk = { "1", "5", "25", "50", "100", "500", "1000" };
                            if (fishk.Contains(update.CallbackQuery.Data))
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                await user.UpdateStageAsync(update.CallbackQuery.Message.Chat.Id, "Buy");

                                var comanda = $"SELECT * FROM acaunt WHERE ID = ?";
                                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                                {

                                    zapros.Parameters.Add("?", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                                    var read = await zapros.ExecuteReaderAsync();
                                    await read.ReadAsync();



                                    int syma = Convert.ToInt32(read[update.CallbackQuery.Data.ToString()]) * Convert.ToInt32(update.CallbackQuery.Data);


                                    await client.SendTextMessageAsync(
                                        chatId: update.CallbackQuery.Message.Chat.Id,
                                        text: $"Ви міняєте фішки номіналом {update.CallbackQuery.Data} \nВиберіть фішку яку купуєте:",
                                        replyMarkup: ButtonBuy(syma, update.CallbackQuery.Data)
                                        );
                                }
                            }
                            else if (update.CallbackQuery.Data == "📄 Назад до профілю")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                await Profil(update.CallbackQuery.Message.Chat.Id);
                            }
                            else
                            {
                                await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"Ви в \"{user.Stage}\" функція \"{update.CallbackQuery.Data}\" недоступна");
                            }
                        }
                        else if (user.Stage == "Buy")
                        {

                            string[] fishk = { "1", "5", "25", "50", "100", "500", "1000" };
                            if (fishk.Contains(update.CallbackQuery.Data))
                            {

                                string text = update.CallbackQuery.Message.Text;

                                string[] parts = text.Split(new string[] { "номіналом" }, StringSplitOptions.None);

                                int twochipValue = Convert.ToInt32(parts[1].Split('\n')[0]);
                                int onechipValue = Convert.ToInt32(update.CallbackQuery.Data);
                                var comanda = $"SELECT * FROM acaunt WHERE ID = ?";
                                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                                {

                                    zapros.Parameters.Add("?", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                                    var read = await zapros.ExecuteReaderAsync();
                                    await read.ReadAsync();

                                    if (twochipValue > onechipValue && (Convert.ToInt32(read[$"{twochipValue}"]) - 1) >= 0)
                                    {
                                        read.Close();

                                        comanda = $"UPDATE acaunt SET `{onechipValue}` = `{onechipValue}` + @number, `{twochipValue}` = `{twochipValue}` - 1 WHERE Id = @ID";
                                        using (MySqlCommand updat = new MySqlCommand(comanda, connection))
                                        {

                                            updat.Parameters.AddWithValue("@number", twochipValue / onechipValue);
                                            updat.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                                            await updat.ExecuteNonQueryAsync();
                                        }
                                        await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"✅Ви поміняли фішку номіналом \"{twochipValue}\"\n на фішки номіналом \"{onechipValue}\".");

                                    }
                                    else if (twochipValue < onechipValue && (Convert.ToInt32(read[$"{twochipValue}"]) - onechipValue / twochipValue) >= 0)
                                    {
                                        read.Close();
                                        comanda = $"UPDATE acaunt SET `{twochipValue}` = `{twochipValue}` - @number, `{onechipValue}` = `{onechipValue}` + 1 WHERE Id = @ID";
                                        using (MySqlCommand updat = new MySqlCommand(comanda, connection))
                                        {

                                            updat.Parameters.AddWithValue("@number", onechipValue / twochipValue);
                                            updat.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                                            await updat.ExecuteNonQueryAsync();
                                        }
                                        await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"✅Ви поміняли фішки номіналом \"{twochipValue}\"\n на фішку номіналом \"{onechipValue}\".");
                                    }
                                    else await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"❌Вам недостатньо фішок номіналом {twochipValue} для здійснення обміну.");
                                }
                            }
                            else if (update.CallbackQuery.Data == "🔄 Обмін фішок")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                                await ChipExchange(update.CallbackQuery.Message.Chat.Id);
                            }
                            else if (update.CallbackQuery.Data == "📄 Назад до профілю")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                await Profil(update.CallbackQuery.Message.Chat.Id);
                            }
                            else
                            {
                                await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"Ви в \"{user.Stage}\" функція \"{update.CallbackQuery.Data}\" недоступна");
                            }
                        }
                        else if (user.Stage == "🂡 Black Jack")
                        {
                            string[] fishk = { "1", "5", "25", "50", "100", "500", "1000" };
                            if (fishk.Contains(update.CallbackQuery.Data))
                            {
                                string comanda = $"SELECT `1` , `5` , `25` , `50` , `100` , `500` , `1000`  FROM acaunt WHERE ID = @ID;";
                                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                                {
                                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;

                                    using (var read = await zapros.ExecuteReaderAsync())
                                    {
                                        read.Read();
                                        if ((int)read[$"{update.CallbackQuery.Data}"] != 0)
                                        {
                                            read.Close();
                                            comanda = $"UPDATE acaunt SET `{update.CallbackQuery.Data}` = `{update.CallbackQuery.Data}` - 1  WHERE ID = @ID";
                                            using (MySqlCommand zap = new MySqlCommand(comanda, connection))
                                            {
                                                zap.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                                                await zap.ExecuteNonQueryAsync();
                                            }

                                            var comand = $"UPDATE Bet SET bet = bet + {update.CallbackQuery.Data}, `{update.CallbackQuery.Data}` = `{update.CallbackQuery.Data}` + 1  WHERE ID = @ID";

                                            using (MySqlCommand za = new MySqlCommand(comand, connection))
                                            {
                                                za.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                                                await za.ExecuteNonQueryAsync();
                                            }


                                            await Bet(update);

                                        }
                                        else
                                        {
                                            await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"❗️У вас закінчилися фішки номіналом {update.CallbackQuery.Data}❗️");
                                        }
                                    }
                                }
                            }
                            else if (update.CallbackQuery.Data == "📃 Назад")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                                Bet bet = new Bet();
                                await bet.clearBet(update.CallbackQuery.Message.Chat.Id);
                                await gamelist(update.CallbackQuery.Message.Chat.Id);
                            }
                            else if (update.CallbackQuery.Data == "🗑️ Очистити")
                            {
                                Bet bet = new Bet();
                                await bet.clearBet(update.CallbackQuery.Message.Chat.Id);
                                await Bet(update);
                            }
                            else if (update.CallbackQuery.Data == "Грати")
                            {
                                BlackJack Game = new BlackJack();
                                await Game.SelectAsync(update.CallbackQuery.Message.Chat.Id);

                                if (Game.card == "")
                                {
                                    Bet bet = new Bet();
                                    await bet.SelectBetAsync(update.CallbackQuery.Message.Chat.Id);
                                    if (bet.bet != 0)
                                    {
                                        await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                                        Random rand = new Random();
                                        List<string> deck = new List<string>() {
                                            "A  ♠","2️⃣♠","3️⃣♠","4️⃣♠","5️⃣♠","6️⃣♠","7️⃣♠","8️⃣♠","9️⃣♠","🔟 ♠","Q  ♠","J  ♠","K  ♠",
                                            "A  ♣","2️⃣♣","3️⃣♣","4️⃣♣","5️⃣♣","6️⃣♣","7️⃣♣","8️⃣♣","9️⃣♣","🔟 ♣","Q  ♣","J  ♣","K  ♣",
                                            "A  ♥","2️⃣♥","3️⃣♥","4️⃣♥","5️⃣♥","6️⃣♥","7️⃣♥","8️⃣♥","9️⃣♥","🔟 ♥","Q  ♥","J  ♥","K  ♥",
                                            "A  ♦","2️⃣♦","3️⃣♦","4️⃣♦","5️⃣♦","6️⃣♦","7️⃣♦","8️⃣♦","9️⃣♦","🔟 ♦","Q  ♦","J  ♦","K  ♦"
                                            };

                                        string card = "";
                                        while (deck.Count != 0)
                                        {
                                            int index = rand.Next(0, deck.Count);
                                            card += deck[index];
                                            deck.RemoveAt(index);
                                        }

                                        string bankcard = card.Substring(0, 4);
                                        card = card.Remove(0, 4);

                                        string playercard1 = card.Substring(0, 4);
                                        card = card.Remove(0, 4);
                                        int playerpoint = CardInPoint(playercard1);

                                        string playercard2 = card.Substring(0, 4);
                                        card = card.Remove(0, 4);
                                        if (playerpoint + CardInPoint(playercard2) > 21) playerpoint = 12;
                                        else playerpoint += CardInPoint(playercard2);

                                        Game.card = card;
                                        Game.bankcard = bankcard;
                                        Game.bankpoint = CardInPoint(bankcard);
                                        Game.playercard = playercard1 + " | " + playercard2;
                                        Game.playerpoint = playerpoint;


                                        await Game.UpdateGameAsync(update.CallbackQuery.Message.Chat.Id);



                                        var Button = new InlineKeyboardMarkup(new[]
                                            {
                                                new[]
                                                {
                                                    InlineKeyboardButton.WithCallbackData("🂠 Hit", "🂠 Hit"),
                                                },
                                                new[]
                                                {
                                                    InlineKeyboardButton.WithCallbackData("🏳️ Surrender", "🏳️ Surrender"),
                                                    InlineKeyboardButton.WithCallbackData("✋ Stand", "✋ Stand")
                                                }
                                            }
                                        );

                                        using (FileStream stream = System.IO.File.OpenRead(@"croupier.jpeg"))
                                        {
                                            await client.SendPhotoAsync(
                                            chatId: update.CallbackQuery.Message.Chat.Id,
                                            photo: new InputFileStream(stream, $"крупє"),
                                            caption: "━━━━━━━━  🎲 Гра в 21 🎲  ━━━━━━━━\n\n" +

                                                $"Дилер: {Game.bankcard} — {Game.bankpoint}\n" +
                                                $"Ви: {Game.playercard} — {Game.playerpoint}",

                                            replyMarkup: Button
                                            );
                                        }
                                    }
                                    else
                                    {
                                        await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, "❗️Ставка не може бути рівна нулю❗️");
                                    }
                                }
                            }
                            else if (update.CallbackQuery.Data == "🏳️ Surrender")
                            {
                                Bet bet = new Bet();
                                await bet.SelectBetAsync(update.CallbackQuery.Message.Chat.Id);

                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);


                                if (bet.bet != 0)
                                {
                                    var comanda = $"UPDATE acaunt SET `1` = `1` + {Math.Floor(Convert.ToDecimal(bet.bet) / 2)} WHERE ID = @ID";
                                    using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                                    {
                                        zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                                        await zapros.ExecuteNonQueryAsync();
                                    }

                                    BlackJack game = new BlackJack();
                                    await game.Reset21gameAsync(update.CallbackQuery.Message.Chat.Id);
                                    await bet.ResetBetAsync(update.CallbackQuery.Message.Chat.Id);

                                    await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Ви здалися, сума вашої ставки була {bet.bet}.\nВи отримали половину назад це {Math.Floor(Convert.ToDecimal(bet.bet) / 2)}");

                                    await gamelist(update.CallbackQuery.Message.Chat.Id);
                                }
                            }
                            else if (update.CallbackQuery.Data == "🂠 Hit")
                            {
                                BlackJack game = new BlackJack();
                                await game.SelectAsync(update.CallbackQuery.Message.Chat.Id);


                                game.playercard = game.playercard + " | " + game.card.Substring(0, 4);
                                game.card = game.card.Remove(0, 4);



                                if (CardInPoint(game.playercard) == 11)
                                {
                                    if (game.playerpoint + 11 > 21)
                                    {
                                        game.playerpoint = game.playerpoint + 1;
                                    }
                                    else
                                    {
                                        game.playerpoint = game.playerpoint + 11;
                                    }
                                }
                                else
                                {
                                    game.playerpoint += CardInPoint(game.playercard);
                                }


                                if (game.playerpoint <= 21)
                                {
                                    await game.UpdateGameAsync(update.CallbackQuery.Message.Chat.Id);

                                    await Tablegame21(update);
                                }
                                else
                                {
                                    Acaunt acaunt = new Acaunt();
                                    Bet bet = new Bet();
                                    await bet.SelectAsync(update.CallbackQuery.Message.Chat.Id);

                                    await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                    await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"У вас перебор:\n" +
                                        $"Дилер: {game.bankcard} — {game.bankpoint}\n" +
                                        $"Ви: {game.playercard} — {game.playerpoint}\n\n" +
                                        $"Ви програли вашу ставку🥲");

                                    await acaunt.UpdateLose(update.CallbackQuery.Message.Chat.Id);
                                    await game.Reset21gameAsync(update.CallbackQuery.Message.Chat.Id);
                                    await bet.ResetBetAsync(update.CallbackQuery.Message.Chat.Id);
                                    await gamelist(update.CallbackQuery.Message.Chat.Id);


                                }
                            }
                            else if (update.CallbackQuery.Data == "✋ Stand")
                            {
                                BlackJack Game = new BlackJack();
                                await Game.SelectAsync(update.CallbackQuery.Message.Chat.Id);

                                while (Game.bankpoint <= 16)
                                {
                                    Game.bankcard = Game.bankcard + " | " + Game.card.Substring(0, 4);
                                    Game.card = Game.card.Remove(0, 4);

                                    if (CardInPoint(Game.bankcard) == 11)
                                    {
                                        if (Game.bankpoint + 11 > 21)
                                        {
                                            Game.bankpoint += 1;
                                        }
                                        else
                                        {
                                            Game.bankpoint += 11;
                                        }
                                    }
                                    else
                                    {
                                        Game.bankpoint += CardInPoint(Game.bankcard);
                                    }
                                }

                                if (Game.bankpoint == Game.playerpoint)
                                {
                                    await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                    await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Нічья, ставка повернулась:\n" +
                                        $"Дилер: {Game.bankcard} — {Game.bankpoint}\n" +
                                        $"Ви: {Game.playercard} — {Game.playerpoint}");
                                    Bet bet = new Bet();
                                    await bet.clearBet(update.CallbackQuery.Message.Chat.Id);
                                    await Game.Reset21gameAsync(update.CallbackQuery.Message.Chat.Id);

                                    await gamelist(update.CallbackQuery.Message.Chat.Id);
                                }
                                else if (Game.playerpoint == 21 && Game.playercard.Length == 11)
                                {
                                    await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                    Acaunt acaunt = new Acaunt();

                                    Bet bet = new Bet();
                                    await bet.SelectBetAsync(update.CallbackQuery.Message.Chat.Id);
                                    await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"🎉 *Вітаємо! У вас Блекджек!* 🎉\n\n" +
                                        $"💼 *Дилер*: {Game.bankcard} — *{Game.bankpoint}*\n" +
                                        $"🃏 *Ви*: {Game.playercard} — *{Game.playerpoint}*\n\n" +
                                        $"💰 *Ваш виграш становить 3:2!*");

                                    int c1000 = 0;
                                    int c500 = 0;
                                    int c100 = 0;
                                    int c50 = 0;
                                    int c25 = 0;
                                    int c5 = 0;
                                    bet.bet = Convert.ToInt32(Math.Floor(bet.bet * 2.5));

                                    while (bet.bet >= 1000)
                                    {
                                        bet.bet -= 1000;
                                        c1000++;
                                    }
                                    while (bet.bet >= 500)
                                    {
                                        bet.bet -= 500;
                                        c500++;
                                    }
                                    while (bet.bet >= 100)
                                    {
                                        bet.bet -= 100;
                                        c100++;
                                    }
                                    while (bet.bet >= 50)
                                    {
                                        bet.bet -= 50;
                                        c50++;
                                    }
                                    while (bet.bet >= 25)
                                    {
                                        bet.bet -= 25;
                                        c25++;
                                    }
                                    while (bet.bet >= 5)
                                    {
                                        bet.bet -= 5;
                                        c5++;
                                    }
                                    string comanda = $"UPDATE acaunt SET `1` = `1` + @C1, `5` = `5` + @C5, `25` = `25` + @C25, `50` = `50` + @C50, `100` = `100` + @C100, `500` = `500` + @C500, `1000` = `1000` + @C1000, Win = Win + 1, Hands = Hands + 1 WHERE ID = @ID;";
                                    using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                                    {
                                        zapros.Parameters.Add("@C1", MySqlDbType.Int32).Value = bet.bet;
                                        zapros.Parameters.Add("@C5", MySqlDbType.Int32).Value = c5;
                                        zapros.Parameters.Add("@C25", MySqlDbType.Int32).Value = c25;
                                        zapros.Parameters.Add("@C50", MySqlDbType.Int32).Value = c50;
                                        zapros.Parameters.Add("@C100", MySqlDbType.Int32).Value = c100;
                                        zapros.Parameters.Add("@C500", MySqlDbType.Int32).Value = c500;
                                        zapros.Parameters.Add("@C1000", MySqlDbType.Int32).Value = c1000;

                                        zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;

                                        await zapros.ExecuteNonQueryAsync();
                                    }


                                    await bet.ResetBetAsync(update.CallbackQuery.Message.Chat.Id);
                                    await Game.Reset21gameAsync(update.CallbackQuery.Message.Chat.Id);

                                    await gamelist(update.CallbackQuery.Message.Chat.Id);
                                }
                                else if (Game.bankpoint > 21)
                                {
                                    await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                    Acaunt acaunt = new Acaunt();

                                    Bet bet = new Bet();

                                    await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"У дилера перебор:\n" +
                                        $"Дилер: {Game.bankcard} — {Game.bankpoint}\n" +
                                        $"Ви: {Game.playercard} — {Game.playerpoint}\n\n" +
                                        $"Ви перемогли ставка удвоїна)🎉");


                                    await acaunt.Update21Win(update.CallbackQuery.Message.Chat.Id);
                                    await bet.ResetBetAsync(update.CallbackQuery.Message.Chat.Id);
                                    await Game.Reset21gameAsync(update.CallbackQuery.Message.Chat.Id);

                                    await gamelist(update.CallbackQuery.Message.Chat.Id);

                                }
                                else if (Game.playerpoint > Game.bankpoint)
                                {

                                    await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                    Acaunt acaunt = new Acaunt();

                                    Bet bet = new Bet();


                                    await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"У вас більше ви перемогли:\n" +
                                        $"Дилер: {Game.bankcard} — {Game.bankpoint}\n" +
                                        $"Ви: {Game.playercard} — {Game.playerpoint}\n" +
                                        $"Ставка удвоїна)");

                                    await acaunt.Update21Win(update.CallbackQuery.Message.Chat.Id);
                                    await bet.ResetBetAsync(update.CallbackQuery.Message.Chat.Id);
                                    await Game.Reset21gameAsync(update.CallbackQuery.Message.Chat.Id);

                                    await gamelist(update.CallbackQuery.Message.Chat.Id);

                                }
                                else if (Game.playerpoint < Game.bankpoint)
                                {

                                    Acaunt acaunt = new Acaunt();
                                    Bet bet = new Bet();
                                    await acaunt.SelectAsync(update.CallbackQuery.Message.Chat.Id);
                                    await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                    await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"У дилера більше ви програли:\n" +
                                        $"Дилер: {Game.bankcard} — {Game.bankpoint}\n" +
                                        $"Ви: {Game.playercard} — {Game.playerpoint}\n\n" +
                                        $"Ставка анульована.");

                                    await acaunt.UpdateLose(update.CallbackQuery.Message.Chat.Id);
                                    await Game.Reset21gameAsync(update.CallbackQuery.Message.Chat.Id);
                                    await bet.ResetBetAsync(update.CallbackQuery.Message.Chat.Id);
                                    await gamelist(update.CallbackQuery.Message.Chat.Id);
                                }
                            }
                            else
                            {
                                await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"Ви в \"{user.Stage}\" функція \"{update.CallbackQuery.Data}\" недоступна");
                            }

                        }
                        else if (user.Stage == "🎰 Classic Slots")
                        {
                            if (update.CallbackQuery.Message.Text[0] != '❌')
                            {

                                if (update.CallbackQuery.Data == "🎲 Крутити")
                                {
                                    _ = Task.Run(() => SpinSlotsAsync(update));

                                }
                                else if (update.CallbackQuery.Data == "🔼 Збільшити ставку")
                                {

                                    bool notvalue = false;
                                    string pattern = @"ставка:\s*(\d+)";

                                    Match search = Regex.Match(update.CallbackQuery.Message.Text, pattern);
                                    int stavka = int.Parse(search.Groups[1].Value);
                                    switch (stavka)
                                    {
                                        case 1: stavka = 5; break;
                                        case 5: stavka = 25; break;
                                        case 25: stavka = 50; break;
                                        case 50: stavka = 100; break;
                                        case 100: stavka = 500; break;
                                        case 500: stavka = 1000; break;
                                        case 1000: notvalue = true; await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, "❗️У вас вже стоїть максимальна ставка❗️"); break;
                                    }
                                    var Buttonslot = new InlineKeyboardMarkup(
                                        new[]
                                        {
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("🔽 Зменшити ставку", "🔽 Зменшити ставку"),
                                            InlineKeyboardButton.WithCallbackData("🔼 Збільшити ставку", "🔼 Збільшити ставку"),

                                        },
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("🎲 Крутити", "🎲 Крутити"),
                                        },
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("📃 Назад", "📃 Назад"),
                                        }
                                        });

                                    if (notvalue == false)
                                    {
                                        await client.EditMessageTextAsync(
                                            chatId: update.CallbackQuery.Message.Chat.Id,
                                            messageId: update.CallbackQuery.Message.MessageId,
                                            text: "------------------------------------------------------------------\n" +
                                                    "|                         | [🍒][🍒][🍒] |                    |\n" +
                                                    "|                      → [7️⃣][7️⃣][7️⃣] ←                 |\n" +
                                                    "|                         | [🍋][🍋][🍋] |                    |\n" +
                                                    "------------------------------------------------------------------\n" +
                                                    $"Ваша ставка: {stavka}\n" +
                                                    "Ваш виіграш: ---------",
                                           replyMarkup: Buttonslot);
                                    }
                                }
                                else if (update.CallbackQuery.Data == "🔽 Зменшити ставку")
                                {
                                    bool notvalue = false;
                                    string pattern = @"ставка:\s*(\d+)";

                                    Match search = Regex.Match(update.CallbackQuery.Message.Text, pattern);
                                    int stavka = int.Parse(search.Groups[1].Value);
                                    switch (stavka)
                                    {
                                        case 1: notvalue = true; await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, "❗️Дії не можливі під час крутіння слотів❗️"); break;
                                        case 5: stavka = 1; break;
                                        case 25: stavka = 5; break;
                                        case 50: stavka = 25; break;
                                        case 100: stavka = 50; break;
                                        case 500: stavka = 100; break;
                                        case 1000: stavka = 500; break;
                                    }

                                    var Buttonslot = new InlineKeyboardMarkup(
                                        new[]
                                        {
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("🔽 Зменшити ставку", "🔽 Зменшити ставку"),
                                            InlineKeyboardButton.WithCallbackData("🔼 Збільшити ставку", "🔼 Збільшити ставку"),

                                        },
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("🎲 Крутити", "🎲 Крутити"),
                                        },
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("📃 Назад", "📃 Назад"),
                                        }
                                        });

                                    if (notvalue == false)
                                    {
                                        await client.EditMessageTextAsync(
                                            chatId: update.CallbackQuery.Message.Chat.Id,
                                            messageId: update.CallbackQuery.Message.MessageId,
                                            text: "------------------------------------------------------------------\n" +
                                                    "|                         | [🍒][🍒][🍒] |                    |\n" +
                                                    "|                      → [7️⃣][7️⃣][7️⃣] ←                 |\n" +
                                                    "|                         | [🍋][🍋][🍋] |                    |\n" +
                                                    "------------------------------------------------------------------\n" +
                                                    $"Ваша ставка: {stavka}\n" +
                                                    "Ваш виіграш: ---------",
                                           replyMarkup: Buttonslot);
                                    }

                                }
                                else if (update.CallbackQuery.Data == "📃 Назад")
                                {
                                    await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);


                                    await gamelist(update.CallbackQuery.Message.Chat.Id);
                                }
                                else
                                {
                                    await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, $"Ви в \"{user.Stage}\" функція \"{update.CallbackQuery.Data}\" недоступна");
                                }
                            }
                            else await client.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id, "❗️Дії не можливі під час крутіння слотів❗️");

                        }
                        else if (user.Stage == "🟥⬛ Roulette")
                        {
                            if (update.CallbackQuery.Data == "📃 Назад")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
                                Bet bet = new Bet();
                                await bet.clearBet(update.CallbackQuery.Message.Chat.Id);
                                Roulette roulette = new Roulette(client, update);
                                await roulette.SelectAsync();
                                roulette.YourBets = "";
                                await roulette.UpdateRouletAsync();
                                await gamelist(update.CallbackQuery.Message.Chat.Id);
                            }
                            else
                            {
                                Roulette roulette = new Roulette(client, update);
                                await roulette.RouletLogic();
                            }
                        }
                        else if (user.Stage == "🎲 Rocket Dice")
                        {
                            if (update.CallbackQuery.Data == "📃 Назад")
                            {
                                await client.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);

                                await gamelist(update.CallbackQuery.Message.Chat.Id);
                            }
                            else
                            {
                                Dice dice = new Dice(client, update);
                                await dice.DiceLogic();
                            }
                        }
                    }
                }
            }
            catch (ApiRequestException ex) when (ex.Message.Contains("query_id_invalid"))
            {
                await Console.Out.WriteLineAsync($"Запит {update.CallbackQuery.Message.Chat.Id} прострочений або недійсний: {ex.Message}");
            }
            catch (ApiRequestException ex) when (ex.Message.Contains("message can't be deleted for everyone"))
            {
                var Error = new InlineKeyboardMarkup(
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("⚠️", "⚠️"),
                                        });

                Bet bet = new Bet();
                await bet.clearBet(update.CallbackQuery.Message.Chat.Id);
                BlackJack blackJack = new BlackJack();
                await blackJack.Reset21gameAsync(update.CallbackQuery.Message.Chat.Id);

                if (update.CallbackQuery.Message.Type == MessageType.Text)
                {   
                    await client.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, "Повідомлення застаріле перейдіть до нового", replyMarkup: Error);
                }
                else 
                {
                    await client.EditMessageCaptionAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, "Повідомлення застаріле перейдіть до нового", replyMarkup: Error);
                }
                
                await Info.Menu(client, update);

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"MCallbackQuery: {update.CallbackQuery.Message.Chat.Id}:: {ex.Message}");
            }
        }

        private static async Task ChipExchange(long chatId)
        {
            Acaunt acaunt = new Acaunt();
            await acaunt.UpdateStageAsync(chatId, "🔄 Обмін фішок");

            await Bot.SendTextMessageAsync(
                chatId: chatId,
                text: "Виберіть тип фішок які міняєте:",
                replyMarkup: await GenerateButton(chatId)
                );
        }
        private static IReplyMarkup ButtonBuy(int syma, string data)
        {
            var button = new List<List<InlineKeyboardButton>>();
            var rad = new List<InlineKeyboardButton>();
            if (syma >= 1 && data != "1")
            {
                rad.Add(InlineKeyboardButton.WithCallbackData("\U0001f7e2 1", "1"));
            }
            if (syma >= 5 && data != "5")
            {
                rad.Add(InlineKeyboardButton.WithCallbackData("🔵 5", "5"));
            }
            if (rad.Count > 0)
            {
                button.Add(rad);
                rad = new List<InlineKeyboardButton>();
            }

            if (syma >= 25 && data != "25")
            {
                rad.Add(InlineKeyboardButton.WithCallbackData("🟠 25", "25"));
            }
            if (syma >= 50 && data != "50")
            {
                rad.Add(InlineKeyboardButton.WithCallbackData("🟡 50", "50"));
            }
            if (rad.Count > 0)
            {
                button.Add(rad);
                rad = new List<InlineKeyboardButton>();
            }

            if (syma >= 100 && data != "100")
            {
                rad.Add(InlineKeyboardButton.WithCallbackData("🔴 100", "100"));
            }
            if (syma >= 500 && data != "500")
            {
                rad.Add(InlineKeyboardButton.WithCallbackData("🟣 500", "500"));
            }
            if (rad.Count > 0)
            {
                button.Add(rad);
                rad = new List<InlineKeyboardButton>();
            }

            if (syma >= 1000 && data != "1000")
            {
                rad.Add(InlineKeyboardButton.WithCallbackData("⚪ 1000", "1000"));
                button.Add(rad);
                rad = new List<InlineKeyboardButton>();
            }

            rad.Add(InlineKeyboardButton.WithCallbackData("🔄 Назад до Обмін фішок", "🔄 Обмін фішок"));
            button.Add(rad);

            rad = new List<InlineKeyboardButton>();
            rad.Add(InlineKeyboardButton.WithCallbackData("📄 Назад до профілю", "📄 Назад до профілю"));
            button.Add(rad);

            var keyboard = new InlineKeyboardMarkup(button);
            return keyboard;
        }
        private static async Task<InlineKeyboardMarkup> GenerateButton(long chatId)
        {
            Acaunt user = new Acaunt();

            await user.SelectAsync(chatId);

            var button = new List<List<InlineKeyboardButton>>();
            var rad = new List<InlineKeyboardButton>();
            if (user.C1 != 0)
            {
                rad.Add(InlineKeyboardButton.WithCallbackData("\U0001f7e2 1", "1"));
            }
            if (user.C5 != 0)
            {
                rad.Add(InlineKeyboardButton.WithCallbackData("🔵 5", "5"));
            }
            if (rad.Count > 0)
            {
                button.Add(rad);
                rad = new List<InlineKeyboardButton>();
            }

            if (user.C25 != 0)
            {
                rad.Add(InlineKeyboardButton.WithCallbackData("\U0001f7e0 25", "25"));
            }
            if (user.C50 != 0)
            {
                rad.Add(InlineKeyboardButton.WithCallbackData("\U0001f7e1 50", "50"));
            }
            if (rad.Count > 0)
            {
                button.Add(rad);
                rad = new List<InlineKeyboardButton>();
            }

            if (user.C100 != 0)
            {
                rad.Add(InlineKeyboardButton.WithCallbackData("🔴 100", "100"));
            }
            if (user.C500 != 0)
            {
                rad.Add(InlineKeyboardButton.WithCallbackData("\U0001f7e3 500", "500"));
            }
            if (rad.Count > 0)
            {
                button.Add(rad);
                rad = new List<InlineKeyboardButton>();
            }

            if (user.C1000 != 0)
            {
                rad.Add(InlineKeyboardButton.WithCallbackData("⚪ 1000", "1000"));
                button.Add(rad);
                rad = new List<InlineKeyboardButton>();
            }

            if (user.Stage == "🔄 Обмін фішок" || user.Stage == "Buy")
            {
                rad.Add(InlineKeyboardButton.WithCallbackData("📄 Назад до профілю", "📄 Назад до профілю"));
                button.Add(rad);
            }
            var keyboard = new InlineKeyboardMarkup(button);
            return keyboard;
        }
        private static async Task Profil(long chatId)
        {
            Acaunt user = new Acaunt();
            await user.SelectAsync(chatId);

            string InterFace;

              
                    InterFace =
                                    $"───────────────────────────\n" +
                                    $"Гравець: {user.NN}\n" +
                                    $"💰 Загальний баланс: {user.C1 + user.C5 * 5 + user.C25 * 25 + user.C50 * 50 + user.C100 * 100 + user.C500 * 500 + user.C1000 * 1000}\n\n" +
                                    $"Фішки:\n";

                    if (user.C1 > 0) InterFace += $"🟢 1       : {user.C1}\n";
                    if (user.C5 > 0) InterFace += $"🔵 5       : {user.C5}\n";
                    if (user.C25 > 0) InterFace += $"🟠 25     : {user.C25}\n";
                    if (user.C50 > 0) InterFace += $"🟡 50     : {user.C50}\n";
                    if (user.C100 > 0) InterFace += $"🔴 100   : {user.C100}\n";
                    if (user.C500 > 0) InterFace += $"🟣 500   : {user.C500}\n";
                    if (user.C1000 > 0) InterFace += $"⚪ 1000 : {user.C1000}\n\n";

                    InterFace += $"Статистика:\n" +
                                  $"🎉 Перемог: \n" +
                                  $"|{line(user.Win, user.Hands)} {user.Win}\n" +
                                  $"😢 Поразок: \n" +
                                  $"|{line(user.Lose, user.Hands)} {user.Lose}\n" +
                                  $"⚖️ Коефіцієнт: {coefficient(user.Win, user.Lose)}\n" +
                                  $"────────────────────────\n";
              
            var ProfilButton = new InlineKeyboardMarkup(
                        new[]{
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("🎲 Ігри", "🎲 Ігри"),
                                InlineKeyboardButton.WithCallbackData("🏆 Рейтинг", "🏆 Рейтинг"),
                            },
                            new[] {
                                InlineKeyboardButton.WithCallbackData("⚙️ Налаштування", "⚙️ Налаштування"),
                                InlineKeyboardButton.WithCallbackData("🔄 Обмін фішок", "🔄 Обмін фішок")
                            },
                            new[] {
                                InlineKeyboardButton.WithCallbackData("Головне меню", "Головне меню"),
                            }
                        });


            await user.UpdateStageAsync(chatId,"📄 Profil");


            await Bot.SendTextMessageAsync(
                        chatId: chatId,
                        text: InterFace,
                        replyMarkup: ProfilButton);
        }

        private static string line(object result, object hands)
        {
            double x = Convert.ToDouble(result);
            double hand = Convert.ToDouble(hands);
            if (x == 0)
            {
                return "";
            }
            else if (x == hand)
            {
                return new string('█', 22);
            }
            else
            {
                return new string('█', (int)Math.Round(x / hand * 22));
            }
        }
        private static async Task Criteri(long chatId, string doptext)
        {
            await Bot.SendTextMessageAsync(chatId,
                                        "Критерії, допустимі символи:\n" +
                                        "• Латинські літери(A-Z, a-z)\n" +
                                        "• Цифри: 0-9\n" +
                                        "• Підкреслення (_), дефіс (-)\n" +
                                        "• Без відступів(пробілів)\n" +
                                        "• Довжина min(3) max(15) символів\n\n" +
                                        $"{doptext}");
        }
        private static string coefficient(object win, object lose)
        {
            double W = Convert.ToDouble(win);
            double L = Convert.ToDouble(lose);

            if (W == 0 || L == 0)
            {
                return "━";
            }
            else
            {
                double coef = W / L;
                return Math.Round(coef, 2).ToString("0.00");
            }
        }
        private static async Task gamelist(long Id)
        {
            Acaunt acaunt = new Acaunt();
            await acaunt.UpdateStageAsync(Id, "🎲 Ігри");
            var Button = new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🂡 Black Jack", "🂡 Black Jack"),
                        InlineKeyboardButton.WithCallbackData("🎰 Classic Slots", "🎰 Classic Slots"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🟥⬛ Roulette", "🟥⬛ Roulette"),
                         InlineKeyboardButton.WithCallbackData("🎲 Rocket Dice", "🎲 Rocket Dice"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🚧 Скоро", "🚧 Скоро"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("📄 Назад до профілю", "📄 Назад до профілю"),
                    }
                });

            using (FileStream stream = System.IO.File.OpenRead("table.jpg"))
            {
                await Bot.SendPhotoAsync(
                    chatId: Id,
                    photo: new InputFileStream(stream, $"game table"),
                    caption: "Оберіть гру:",
                    replyMarkup: Button);
            }
        }


        private static async Task Bet(Update update)
        {
            Acaunt acaunt = new Acaunt();
            await acaunt.SelectStageAsync(update.CallbackQuery.Message.Chat.Id);
            Bet bet = new Bet();
            await bet.SelectAsync(update.CallbackQuery.Message.Chat.Id);

            var Button = new InlineKeyboardMarkup(
                new[]
                {
                         InlineKeyboardButton.WithCallbackData("📃 Назад", "📃 Назад"),
                         InlineKeyboardButton.WithCallbackData("🗑️ Очистити", "🗑️ Очистити"),
                         InlineKeyboardButton.WithCallbackData("Грати", "Грати"),

                }
            );
            await Bot.EditMessageCaptionAsync(
                chatId: update.CallbackQuery.Message.Chat.Id,
                messageId: update.CallbackQuery.Message.MessageId,
                caption: $"━━━━━━━━━ {acaunt.Stage} ━━━━━━━━━\n" +
                 $"Ваша ставка: {bet.bet}",
                replyMarkup: new InlineKeyboardMarkup((await GenerateButton(update.CallbackQuery.Message.Chat.Id)).InlineKeyboard.Concat(Button.InlineKeyboard).ToArray())
                );
        }
        
        private static int CardInPoint(string card)
        {
            int point = 0;

            switch (card[card.Length - 4])
            {
                case '9': point = 9; break;
                case '8': point = 8; break;
                case '7': point = 7; break;
                case '6': point = 6; break;
                case '5': point = 5; break;
                case '4': point = 4; break;
                case '3': point = 3; break;
                case '2': point = 2; break;
                case 'A': point = 11; break;
                default:
                    point = 10; break;
            }
            return point;
        }
        private static async Task Tablegame21(Update update)
        {
            BlackJack game = new BlackJack();
            await game.SelectAsync(update.CallbackQuery.Message.Chat.Id);


            var Button = new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🂠 Hit", "🂠 Hit"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("✋ Stand", "✋ Stand")
                    }
                }
            );


            await Bot.EditMessageCaptionAsync(
                chatId: update.CallbackQuery.Message.Chat.Id,
                messageId: update.CallbackQuery.Message.MessageId,
                caption: "━━━━━━━━  🎲 Гра в 21 🎲  ━━━━━━━━\n\n" +

                    $"Дилер: {game.bankcard} — {game.bankpoint}\n" +
                    $"Ви: {game.playercard} — {game.playerpoint}",

                replyMarkup: Button
            );
        }



        private static async Task SpinSlotsAsync(Update update)
        {
            try
            {
                
                
    
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    int stavkalose = 1;
                    string pattern = @"(?<=Ваша ставка:\s*)[^\r\n]+";
                    ;
                    int free = 0;

                    Match match = Regex.Match(update.CallbackQuery.Message.Text, pattern);
                    string stavka = match.Value.Replace(" ", "");

                    if (stavka.Contains("FreeSpine"))
                    {
                        stavkalose = 0;
                        stavka = stavka.Replace("FreeSpine", "");

                        int delimiterIndex = stavka.IndexOf('x');
                        free = Convert.ToInt32(stavka.Substring(delimiterIndex + 1).Trim());
                        free--;
                        stavka = stavka.Substring(0, delimiterIndex).Trim();
                    }

                    string comanda = $"SELECT `{stavka}` FROM acaunt WHERE ID = @ID;";
                    using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                    {
                        zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;

                        using (var read = await zapros.ExecuteReaderAsync())
                        {

                            read.Read();

                            if ((int)read[$"{stavka}"] != 0 || stavkalose == 0)
                            {
                                string[] reel = { "💎", "🍒", "🍋", "🍉", "⭐", "🎰", "7️⃣", "💎", "🍒" };
                                string[] reel1 = { "🍒", "🍒", "🍒", "🍒", "🍒", "🍒", "🍒", "🍒", "🍒", "🍒", "🍒", "🍒", "🍒", "🍒", "🎰", "🎰", "🎰", "🎰", "🎰", "🎰", "🎰", "💎", "7️⃣", "7️⃣", "7️⃣", "7️⃣", "7️⃣", "⭐", "⭐", "⭐", "⭐", "⭐", "🍉", "🍉", "🍉", "🍉", "🍉", "🍉", "🍉", "🍉", "🍋", "🍋", "🍋", "🍋", "🍋", "🍋", "🍋", "🍋", "🍋", "🍋" };

                                Random random = new Random();
                                string finishslot1 = reel1[random.Next(0, reel1.Length)];
                                string finishslot2 = reel1[random.Next(0, reel1.Length)];
                                string finishslot3 = reel1[random.Next(0, reel1.Length)];
                                int slot1 = random.Next(1, reel.Length - 1);
                                int slot2 = random.Next(1, reel.Length - 1);
                                int slot3 = random.Next(1, reel.Length - 1);

                                var Buttonslot = new InlineKeyboardMarkup(
                                    new[]
                                    {
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("🔽 Зменшити ставку", "🔽 Зменшити ставку"),
                                            InlineKeyboardButton.WithCallbackData("🔼 Збільшити ставку", "🔼 Збільшити ставку"),
                                        },
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("🎲 Крутити", "🎲 Крутити"),
                                        },
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("📃 Назад", "📃 Назад"),
                                        }
                                    });

                                while (reel[slot1] != finishslot1 || reel[slot2] != finishslot2 || reel[slot3] != finishslot3)
                                {

                                    if (reel[slot1] != finishslot1)
                                    {
                                        slot1++;
                                        slot2++;
                                        slot3++;
                                        if (slot1 >= reel.Length - 1) slot1 = 1;
                                        if (slot2 >= reel.Length - 1) slot2 = 1;
                                        if (slot3 >= reel.Length - 1) slot3 = 1;
                                    }
                                    else
                                    {
                                        if (reel[slot2] != finishslot2)
                                        {
                                            slot2++;
                                            slot3++;
                                            if (slot2 >= reel.Length - 1) slot2 = 1;
                                            if (slot3 >= reel.Length - 1) slot3 = 1;
                                        }
                                        else
                                        {
                                            if (reel[slot3] != finishslot3)
                                            {
                                                slot3++;
                                                if (slot3 >= reel.Length - 1) slot3 = 1;
                                            }
                                        }
                                    }

                                    await Task.Delay(300);
                                    await Bot.EditMessageTextAsync(
                                         chatId: update.CallbackQuery.Message.Chat.Id,
                                         messageId: update.CallbackQuery.Message.MessageId,
                                         text: "❌\n" +
                                                  "------------------------------------------------------------------\n" +
                                                 $"|                         | [{reel[slot1 - 1]}][{reel[slot2 - 1]}][{reel[slot3 - 1]}] |                    |\n" +
                                                 $"|                      → [{reel[slot1]}][{reel[slot2]}][{reel[slot3]}] ←                 |\n" +
                                                 $"|                         | [{reel[slot1 + 1]}][{reel[slot2 + 1]}][{reel[slot3 + 1]}] |                    |\n" +
                                                 "------------------------------------------------------------------\n" +

                                                 $"Виграшна комбінація: \n" + 
                                                 $"Ваша ставка: \n" +
                                                 "Ваш виіграш: ---------",
                                         replyMarkup: Buttonslot);
                                }

                                string combinate = "-----";
                                int win=0;
                                string itog = "Lose";
                                if (reel[slot1] == reel[slot2] && reel[slot1] == reel[slot3])
                                {
                                    switch (reel[slot1])
                                    {
                                        case "🍒": win = 5; break;
                                        case "🍉": win = 8; break;
                                        case "🍋": win = 10; break;
                                        case "⭐": win = 15; break;
                                        case "7️⃣": win = 20; break;
                                        case "💎": win = 50; break;
                                        case "🎰": free = 10; break;
                                    }
                                    itog = "Win";
                                    combinate = reel[slot1] + reel[slot2] + reel[slot3];

                                }
                                else if (reel[slot1] == reel[slot2] || reel[slot1] == reel[slot3] || reel[slot2] == reel[slot3] )
                                {
                                    string slot = "";
                                    if ((reel[slot1] == reel[slot2] || reel[slot1] == reel[slot3]) && reel[slot1]!= "💎")
                                    {
                                        itog = "Win";
                                        slot = reel[slot1];
                                        combinate = reel[slot1] + reel[slot1];
                                    }
                                    else if ((reel[slot2] == reel[slot1] || reel[slot2] == reel[slot3]) && reel[slot2] != "💎")
                                    {
                                        itog = "Win";
                                        slot = reel[slot2];
                                        combinate = reel[slot2] + reel[slot2];
                                    }

                                    switch (slot)
                                    {
                                        case "🍒": win = 1; break;
                                        case "🍉": win = 2; break;
                                        case "🍋": win = 2; break;
                                        case "⭐": win = 3; break;
                                        case "7️⃣": win = 5; break;
                                        case "🎰": free = 2; break;
                                    }
                                }
                                
                                comanda = $"UPDATE acaunt SET `{stavka}` = `{stavka}` + {win} - {stavkalose}, `Hands` = `Hands` + 1, `{itog}` = `{itog}` + 1 where ID = @ID;";

                                win = Convert.ToInt32(stavka) * win;

                                if (free != 0) stavka = stavka + "x" + free + "FreeSpine";

                                await Bot.EditMessageTextAsync(
                                         chatId: update.CallbackQuery.Message.Chat.Id,
                                         messageId: update.CallbackQuery.Message.MessageId,
                                         text: "------------------------------------------------------------------\n" +
                                                $"|                         | [{reel[slot1 - 1]}][{reel[slot2 - 1]}][{reel[slot3 - 1]}] |                    |\n" +
                                                $"|                      → [{reel[slot1]}][{reel[slot2]}][{reel[slot3]}] ←                 |\n" +
                                                $"|                         | [{reel[slot1 + 1]}][{reel[slot2 + 1]}][{reel[slot3 + 1]}] |                    |\n" +
                                                "------------------------------------------------------------------\n" +
                                                $"Виграшна комбінація: {combinate}\n" +
                                                $"Ваша ставка: {stavka}\n" +
                                                $"Ваш виіграш: {win}\n",
                                                
                                         replyMarkup: Buttonslot);



                            }
                            else await Bot.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, $"Фішок номіналом {stavka} недостатньо.");
                        }
                        using (var connectio = new MySqlConnection(connectionString))
                        {
                            await connectio.OpenAsync();


                            using (MySqlCommand zapro = new MySqlCommand(comanda, connection))
                            {
                                zapro.Parameters.Add("@ID", MySqlDbType.Int64).Value = update.CallbackQuery.Message.Chat.Id;
                                await zapro.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

    }
}
