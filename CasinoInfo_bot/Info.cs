using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CasinoInfo_bot
{

    internal static class Info
    {
        
        private static InlineKeyboardMarkup Button = new InlineKeyboardMarkup(
                                    new[]
                                    {
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("⬅️", "⬅️"),
                                            InlineKeyboardButton.WithCallbackData("➡️", "➡️"),
                                        },
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("Головне меню", "Головне меню"),
                                        }
                                    });
        private static InlineKeyboardMarkup ButtonLeft = new InlineKeyboardMarkup(
                                     new[]
                                     {
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("⬅️", "⬅️"),
                                        },
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("Головне меню", "Головне меню"),
                                        }
                                     });
        private static InlineKeyboardMarkup ButtonRight = new InlineKeyboardMarkup(
                                    new[]
                                    {
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("➡️", "➡️"),
                                        },
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("Головне меню", "Головне меню"),
                                        }
                                    });


        internal static async Task Menu(ITelegramBotClient Bot, Telegram.Bot.Types.Update update)
        {
            Acaunt user = new Acaunt();

           
            var keyboard = new InlineKeyboardMarkup(new[]
                                {
                                new []
                                    {
                                        InlineKeyboardButton.WithCallbackData("Інформація про казино", "info_casino")
                                    },
                                new []
                                    {
                                        InlineKeyboardButton.WithCallbackData("Інформація про ігри", "info_games"),
                                    },
                                new []
                                    {
                                        InlineKeyboardButton.WithCallbackData("Поради щодо безпеки", "safety_tips"),
                                    },
                                new []
                                    {
                                        InlineKeyboardButton.WithCallbackData("🎮 Демо гра", "Profil")
                                    }
                                });
            await user.UpdateStageAsync(update.CallbackQuery.Message.Chat.Id, "Головне меню");
            await Bot.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Головне меню\nОберіть що вас цікавить.", replyMarkup: keyboard);
        }

        private static string casinoinfo1 = "1 / 9\n<b>    Що таке казино? </b>\n<i>   Казино </i> — це заклад, де люди можуть брати участь в азартних іграх 🎲, роблячи ставки 💸 на різні види ігор.\n   Казино бувають як фізичні 🏢, так і онлайн 🌐, але суть у них однакова: гравці намагаються виграти 💰, роблячи ставки на ігри з випадковим результатом 🎰.";
        private static string casinoinfo2 = "2 / 9\n<b>    Як працює казино? 🎰</b>\n   Кожна гра в казино спроектована так, щоб приносити<b> прибуток</b> закладу 💵. Це досягається за допомогою концепції<i> \"house edge\" (перевага казино) 🎲</i>. Це означає, що ймовірність виграшу завжди трохи менша для гравця, ніж здається. Хоча окремі гравці можуть вигравати великі суми 🏆, у<u> довгостроковій перспективі</u> казино завжди виграє більше 💰.";
        private static string casinoinfo3 = "3 / 9\n<b>    Чому люди програють у казино? 🤔</b>\n   Азартні ігри базуються на випадковості 🎲, і гравці не можуть контролювати результат. Незважаючи на це, багато людей вірять у <i>\"системи\"</i> або<i>\"стратегії\"</i>, що, як їм здається, можуть підвищити їхні шанси на виграш 🎯. Однак ці стратегії не можуть обійти математику казино 📊. Чим більше часу людина проводить в азартній грі, тим більше шансів програти ⏳.";
        private static string casinoinfo4 = "4 / 9\n<b>    Психологічні трюки казино</b> \U0001f9e0\r\n   Казино застосовують безліч психологічних прийомів, щоб змусити людей грати довше:\r\n⏳   <b>Відсутність вікон і годинників:</b> Щоб гравці втрачали відчуття часу.\r\n✨   <b>Яскраві вогні та звуки:</b> Ці стимули створюють атмосферу перемог і спонукають гравців грати далі.\r\n🎰   <b>\"Майже виграш\":</b> Багато ігрових автоматів налаштовані так, щоб показувати часті \"майже виграші\", що стимулює гравців робити нові ставки.";
        private static string casinoinfo5 = "5 / 9\n<b>    Фінансові наслідки азартних ігор 💸\n</b>   Азартні ігри можуть призвести до значних фінансових втрат 📉. Люди часто починають грати з маленьких сум 💵, але з часом можуть втратити контроль і ставити все більше грошей 💰. Це може призвести до<b>боргів</b>, втрати<u> заощаджень</u> та навіть <i>майна</i> 🏠. Деякі гравці беруть кредити 💳, сподіваючись \"відігратися\", що лише погіршує їхнє фінансове становище 📊.";
        private static string casinoinfo6 = "6 / 9\n<b>    Залежність від азартних ігор</b> 🎲\r\n   Азартні ігри можуть викликати залежність, подібну до залежності від алкоголю чи наркотиків 🍷💊. Коли людина виграє, у її мозку виробляється дофамін — \"гормон задоволення\" 😃. Це створює сильне бажання продовжувати грати, навіть якщо людина програє 💔. Вона намагається \"відігратися\", що призводить до ще більших втрат 📉.";
        private static string casinoinfo7 = "7 / 9\n<b>    Онлайн-казино</b> 💻\n   З розвитком інтернету азартні ігри стали ще доступнішими через онлайн-казино 🌐. Вони працюють 24/7 🕒 і доступні з будь-якого пристрою 📱💻, що збільшує ризик втратити контроль над грою. Онлайн-казино використовують ті ж методи, що і фізичні казино, для утримання гравців, включаючи бонуси 🎁, безкоштовні спіни 🔄 та різні акції 🎉.";
        private static string casinoinfo8 = "8 / 9\n<b>    Соціальні наслідки азартних ігор ⚖️\n   </b>Азартні ігри не тільки завдають фінансової шкоди 💸, але й впливають на стосунки 💔, роботу 🏢 та здоров’я гравців 🧠. Залежність від азартних ігор може призвести до конфліктів у сім'ї 👨‍👩‍👦, втрати роботи 📉, депресії 😔 та ізоляції 🚪. Багато залежних гравців стикаються з соціальною стигматизацією і не отримують необхідної підтримки 🤝.";
        private static string casinoinfo9 = "9 / 9\n<b>    Як уникнути проблем з азартними іграми? 🚨\n   </b>⏳💸 <b>Обмежуйте час і гроші:</b> Завжди визначайте ліміти на час і гроші, які ви готові витратити.\n   🎯 <b> Грайте лише для розваги:</b> Ставтесь до азартних ігор як до розваги, а не способу заробітку. \n   💼❌ <b>Уникайте гри, якщо у вас є фінансові труднощі:</b> Якщо ви маєте борги або фінансові проблеми, азартні ігри не є рішенням. \n   🤝 <b>Звертайтеся по допомогу:</b> Якщо ви помітили, що грати стало важко контролювати, зверніться до фахівців або підтримуючих груп.";

        internal static async Task SendCasinoInfoAsync(ITelegramBotClient Bot, Telegram.Bot.Types.Update update)
        {
            
            Acaunt acaunt = new Acaunt();
            await acaunt.UpdateStageAsync(update.CallbackQuery.Message.Chat.Id,"CasinoInfo");
            await Bot.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);


            using (FileStream stream = System.IO.File.OpenRead(@"CasinoInfo\1.jpg"))
            {
                await Bot.SendPhotoAsync(
                    chatId: update.CallbackQuery.Message.Chat.Id,
                    photo: new InputFileStream(stream, $"1"),
                    caption: casinoinfo1,
                    parseMode: ParseMode.Html,
                    replyMarkup: ButtonRight
                );
            }
             
        }


        internal static async Task MoveRightCasinoInfo(ITelegramBotClient Bot, Telegram.Bot.Types.Update update)
        {
            char number = update.CallbackQuery.Message.Caption[0];

            string text  = "";

            switch (number)
            {
                case '1':text = casinoinfo2;break;
                case '2': text = casinoinfo3; break;
                case '3': text = casinoinfo4; break;
                case '4': text = casinoinfo5; break;
                case '5': text = casinoinfo6; break;
                case '6': text = casinoinfo7; break;
                case '7': text = casinoinfo8; break;
                case '8': text = casinoinfo9;Button = ButtonLeft;break;
            }
            number = Convert.ToChar(Convert.ToInt32(number)+1);

            //await Bot.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
            
            using (FileStream stream = System.IO.File.OpenRead($@"CasinoInfo\{number}.jpg"))
            {
                await Bot.EditMessageMediaAsync(
                    messageId: update.CallbackQuery.Message.MessageId,
                    chatId: update.CallbackQuery.Message.Chat.Id,
                    media : new InputMediaPhoto(new InputFileStream(stream, $"{number}"))
                    {
                        Caption = text,
                        ParseMode = ParseMode.Html
                    },
                    replyMarkup: Button
                );
            }
        }
        internal static async Task MoveLeftCasinoInfo(ITelegramBotClient Bot, Telegram.Bot.Types.Update update)
        {
            char number = update.CallbackQuery.Message.Caption[0];

            string text = "";

            switch (number)
            {
                case '9': text = casinoinfo8; break;
                case '8': text = casinoinfo7; break;
                case '7': text = casinoinfo6; break;
                case '6': text = casinoinfo5; break;
                case '5': text = casinoinfo4; break;
                case '4': text = casinoinfo3; break;
                case '3': text = casinoinfo2; break;
                case '2': text = casinoinfo1; Button = ButtonRight; break;
            }
            number = Convert.ToChar(Convert.ToInt32(number) - 1);

            using (FileStream stream = System.IO.File.OpenRead($@"CasinoInfo\{number}.jpg"))
            {
                await Bot.EditMessageMediaAsync(
                    messageId: update.CallbackQuery.Message.MessageId,
                    chatId: update.CallbackQuery.Message.Chat.Id,
                    media: new InputMediaPhoto(new InputFileStream(stream, $"{number}"))
                    {
                        Caption = text,
                        ParseMode = ParseMode.Html
                    },
                    replyMarkup: Button
                );
            }
        }

        private static string casinogameinfo1 = "1 / 6\n<b>    Слоти (ігрові автомати)</b> 🎰 \r\n  Це найбільш поширена гра в онлайн та офлайн казино. Гравець робить ставку, натискає кнопку або тягне за важіль, після чого барабани починають обертатися. Завдання гравця — зібрати виграшну комбінацію символів на активних лініях виплат. \r\n  <b>Особливості:</b> 🎉 різноманітність тем, бонуси, джекпоти, безкоштовні обертання. ";
        private static string casinogameinfo2 = "2 / 6\n<b>    Рулетка</b> 🎡 \r\n  Гра, де гравці роблять ставки на числа або їх комбінації, на кольори (червоне або чорне) чи інші варіанти. Після того як всі ставки зроблені, дилер обертає колесо, і кулька падає на певне число. Виграють ті гравці, чиї ставки співпали з результатом. 🎯 \r\n  <b>Типи ставок:</b> внутрішні (на окремі числа), зовнішні (на групи чисел), червоне/чорне, парне/непарне.";
        private static string casinogameinfo3 = "3 / 6\n<b>    Блекджек</b> 🃏 \r\n  Це карткова гра, де гравці змагаються з дилером. Мета — набрати суму очок, близьку до 21, але не перевищити це число. Карти з 2 до 10 мають своє номінальне значення, а король, дама, валет — по 10 очок. Туз може бути 1 або 11 очок. Гравець може \"взяти\" додаткові карти або \"зупинитися\", якщо вважає, що його рука сильна. \r\n  <b>Особливості:</b> швидкі рішення, стратегія \"подвійного шансу\" (Double Down), можливість \"розділення\" пар (Split).";
        private static string casinogameinfo4 = "4 / 6\n<b>    Покер</b> ♠️ \r\n  Гравці отримують карти та роблять ставки на свої комбінації, намагаючись створити найкращу руку або змусити інших гравців скинути карти. Найпопулярніший варіант — Техаський Холдем, де кожен гравець отримує дві закриті карти, а ще п'ять карт розкриваються на столі. \r\n  <b>Особливості:</b> блеф, різні типи покеру (Омаха, Стад), турніри з великими призами.";
        private static string casinogameinfo5 = "5 / 6\n<b>    Баккара</b> 🎴 \r\n  Ця гра відома своєю простотою та швидким темпом. Гравці роблять ставки на руку \"гравця\", \"банкіра\" або на \"нічию\". Мета — отримати суму очок, ближчу до 9. Усі десятки і карти з малюнками мають значення 0, туз — 1, а решта карт мають своє номінальне значення. Якщо сума очок перевищує 9, відкидається перша цифра (наприклад, 15 стає 5). \r\n  <b>Особливості:</b> низька перевага казино на ставці на банкіра, простота правил.";
        private static string casinogameinfo6 = "6 / 6\n<b>    Кості (Крепс)</b> 🎲 \r\n  Гравці роблять ставки на результат кидання двох кубиків. Основна ставка — це \"Pass Line\", де гравець виграє, якщо під час першого кидка випаде 7 або 11. Якщо випадає 2, 3 або 12, гравець програє. Якщо випадає будь-яке інше число (4, 5, 6, 8, 9 або 10), це стає \"поінтом\", і гравець продовжує кидати кубики, поки знову не випаде поінт або 7 (що закінчує гру). \r\n  <b>Особливості:</b> швидкий темп, багато видів ставок, великий азарт.";
        internal static async Task SendCasinoGameInfoAsync(ITelegramBotClient Bot, Telegram.Bot.Types.Update update)
        {
            Acaunt acaunt = new Acaunt();
            await acaunt.UpdateStageAsync(update.CallbackQuery.Message.Chat.Id, "CasinoGameInfo");
            await Bot.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);


            using (FileStream stream = System.IO.File.OpenRead(@"CasinoGameInfo\1.jpg"))
            {
                await Bot.SendPhotoAsync(
                    chatId: update.CallbackQuery.Message.Chat.Id,
                    photo: new InputFileStream(stream, $"1"),
                    caption: casinogameinfo1,
                    parseMode: ParseMode.Html,
                    replyMarkup: ButtonRight
                );
            }
        }

        internal static async Task MoveRightCasinoGameInfo(ITelegramBotClient Bot, Telegram.Bot.Types.Update update)
        {
            char number = update.CallbackQuery.Message.Caption[0];

            string text = "";

            switch (number)
            {
                case '1': text = casinogameinfo2; break;
                case '2': text = casinogameinfo3; break;
                case '3': text = casinogameinfo4; break;
                case '4': text = casinogameinfo5; break;
                case '5': text = casinogameinfo6; Button = ButtonLeft; break;
            }
            number = Convert.ToChar(Convert.ToInt32(number) + 1);

            using (FileStream stream = System.IO.File.OpenRead($@"CasinoGameInfo\{number}.jpg"))
            {
                await Bot.EditMessageMediaAsync(
                    messageId: update.CallbackQuery.Message.MessageId,
                    chatId: update.CallbackQuery.Message.Chat.Id,
                    media: new InputMediaPhoto(new InputFileStream(stream, $"{number}"))
                    {
                        Caption = text,
                        ParseMode = ParseMode.Html
                    },
                    replyMarkup: Button
                );
            }
        }

        internal static async Task MoveLeftCasinoGameInfo(ITelegramBotClient Bot, Telegram.Bot.Types.Update update)
        {

            char number = update.CallbackQuery.Message.Caption[0];

            string text = "";

            switch (number)
            {
                case '6': text = casinogameinfo5; break;
                case '5': text = casinogameinfo4; break;
                case '4': text = casinogameinfo3; break;
                case '3': text = casinogameinfo2; break;
                case '2': text = casinogameinfo1; Button = ButtonRight; break;
            }
            number = Convert.ToChar(Convert.ToInt32(number) - 1);

            using (FileStream stream = System.IO.File.OpenRead($@"CasinoGameInfo\{number}.jpg"))
            {
                await Bot.EditMessageMediaAsync(
                    messageId: update.CallbackQuery.Message.MessageId,
                    chatId: update.CallbackQuery.Message.Chat.Id,
                    media: new InputMediaPhoto(new InputFileStream(stream, $"{number}"))
                    {
                        Caption = text,
                        ParseMode = ParseMode.Html
                    },
                    replyMarkup: Button
                );
            }
        }
        private static string casinosafetyinfo1 = "1 / 5\n<b>Контроль бюджету</b> 💰\r\n- <b>Встановіть ліміт на витрати:</b> Перед початком гри визначте суму, яку ви готові витратити, і ніколи не перевищуйте цей ліміт.\r\n- <b>Не грайте на позичені гроші:</b> Завжди грайте тільки на власні кошти, які ви готові втратити без шкоди для свого фінансового становища.\r\n- <b>Уникайте погоні за виграшем:</b> Якщо ви програли, не намагайтеся негайно відігратися. Це часто призводить до ще більших втрат.\r\n";
        private static string casinosafetyinfo2 = "2 / 5\n<b>Вибір надійних платформ</b> 🏛️\r\n- <b>Грайте в ліцензованих казино:</b> Якщо ви граєте онлайн, вибирайте лише перевірені казино з ліцензією та хорошою репутацією. Це гарантує, що гра буде чесною, а ваші дані будуть захищені.\r\n- <b>Перевіряйте відгуки та рейтинги:</b> Перед реєстрацією на платформі почитайте відгуки інших користувачів, щоб уникнути шахрайських сайтів.\r\n";
        private static string casinosafetyinfo3 = "3 / 5\n<b>Захист особистих даних</b> 🔒\r\n- <b>Ніколи не передавайте свої паролі або дані кредитних карт третім особам:</b> Для онлайн-казино використовуйте сильний пароль і уникайте збереження даних на загальнодоступних комп'ютерах.\r\n- <b>Використовуйте двофакторну аутентифікацію:</b> Багато надійних платформ пропонують цей спосіб для додаткового захисту акаунта.\r\n";
        private static string casinosafetyinfo4 = "4 / 5\n<b>Відстежуйте час</b> ⏰\r\n- <b>Установіть ліміти часу:</b> Встановіть часові рамки для гри і дотримуйтесь їх, щоб уникнути надмірної гри, яка може призвести до втрати контролю над витратами.\r\n- <b>Робіть перерви:</b> Під час гри важливо робити регулярні перерви, щоб уникнути перевтоми або втрати концентрації.\r\n";
        private static string casinosafetyinfo5 = "5 / 5\n<b>Остерігайтеся шахрайства</b> ⚠️\r\n- <b>Уникайте пропозицій про легкі виграші:</b> Якщо ви зустрічаєте пропозиції, які обіцяють надзвичайно високі виграші або бонуси, це може бути ознакою шахрайства.\r\n- <b>Не завантажуйте підозрілі програми:</b> Для гри на мобільних пристроях завантажуйте додатки тільки з офіційних магазинів (App Store, Google Play) або офіційних сайтів казино.\r\n- <b>Читайте умови акцій:</b> Багато казино пропонують бонуси, але вони можуть мати складні умови відіграшу. Переконайтеся, що ви розумієте, як використовувати бонуси і що вам потрібно зробити для їх відіграшу.\r\n";
        internal static async Task SendCasinoSafetyInfoAsync(ITelegramBotClient Bot, Telegram.Bot.Types.Update update)
        {
            Acaunt acaunt = new Acaunt();
            await acaunt.UpdateStageAsync(update.CallbackQuery.Message.Chat.Id, "CasinoSafetyInfo");
            await Bot.DeleteMessageAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);


            using (FileStream stream = System.IO.File.OpenRead(@"CasinoSafetyInfo\1.jpg"))
            {
                await Bot.SendPhotoAsync(
                    chatId: update.CallbackQuery.Message.Chat.Id,
                    photo: new InputFileStream(stream, $"1"),
                    caption: casinosafetyinfo1,
                    parseMode: ParseMode.Html,
                    replyMarkup: ButtonRight
                );
            }

        }
        internal static async Task MoveRightCasinoSafetyInfo(ITelegramBotClient Bot, Telegram.Bot.Types.Update update)
        {
            char number = update.CallbackQuery.Message.Caption[0];

            string text = "";

            switch (number)
            {
                case '1': text = casinosafetyinfo2; break;
                case '2': text = casinosafetyinfo3; break;
                case '3': text = casinosafetyinfo4; break;
                case '4': text = casinosafetyinfo5; Button = ButtonLeft; break;
            }
            number = Convert.ToChar(Convert.ToInt32(number) + 1);

            using (FileStream stream = System.IO.File.OpenRead($@"CasinoSafetyInfo\{number}.jpg"))
            {
                await Bot.EditMessageMediaAsync(
                    messageId: update.CallbackQuery.Message.MessageId,
                    chatId: update.CallbackQuery.Message.Chat.Id,
                    media: new InputMediaPhoto(new InputFileStream(stream, $"{number}"))
                    {
                        Caption = text,
                        ParseMode = ParseMode.Html
                    },
                    replyMarkup: Button
                );
            }
        }
        internal static async Task MoveLeftCasinoSafetyInfo(ITelegramBotClient Bot, Telegram.Bot.Types.Update update)
        {

            char number = update.CallbackQuery.Message.Caption[0];

            string text = "";

            switch (number)
            {
                case '5': text = casinosafetyinfo4; break;
                case '4': text = casinosafetyinfo3; break;
                case '3': text = casinosafetyinfo2; break;
                case '2': text = casinosafetyinfo1; Button = ButtonRight; break;
            }
            number = Convert.ToChar(Convert.ToInt32(number) - 1);

            using (FileStream stream = System.IO.File.OpenRead($@"CasinoSafetyInfo\{number}.jpg"))
            {
                await Bot.EditMessageMediaAsync(
                    messageId: update.CallbackQuery.Message.MessageId,
                    chatId: update.CallbackQuery.Message.Chat.Id,
                    media: new InputMediaPhoto(new InputFileStream(stream, $"{number}"))
                    {
                        Caption = text,
                        ParseMode = ParseMode.Html
                    },
                    replyMarkup: Button
                );
            }
        }
    }
}
