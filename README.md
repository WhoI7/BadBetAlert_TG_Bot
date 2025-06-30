Відновлення проги:

1)Скачати архів
2)В MySQL створити базу данних запросом:

Поля у вигялді цифр повинні бути обгорнуті лапками -> `

CREATE DATABASE IF NOT EXISTS GameData1;
USE GameData;
CREATE TABLE acaunt (
    ID BIGINT PRIMARY KEY,
    NN VARCHAR(50),
    Stage VARCHAR(20),
    `1` INT DEFAULT 1,
    `5` INT DEFAULT 1,
    `25` INT DEFAULT 1,
    `50` INT DEFAULT 1,
    `100` INT DEFAULT 1,
    `500` INT DEFAULT 1,
    `1000` INT DEFAULT 1,
    Hands INT DEFAULT 0,
    Win INT DEFAULT 0,
    Lose INT DEFAULT 0
);
CREATE TABLE `21game` (
    ID BIGINT PRIMARY KEY,
    card VARCHAR(255),
    bankcard VARCHAR(255),
    bankpoint INT DEFAULT 0,
    playercard VARCHAR(255),
    playerpoint INT DEFAULT 0
);
CREATE TABLE bet (
    ID BIGINT PRIMARY KEY,
    bet INT DEFAULT 0,
    `1` INT DEFAULT 0,
    `5` INT DEFAULT 0,
    `25` INT DEFAULT 0,
    `50` INT DEFAULT 0,
    `100` INT DEFAULT 0,
    `500` INT DEFAULT 0,
    `1000` INT DEFAULT 0
);
CREATE TABLE roulette (
    ID BIGINT PRIMARY KEY,
    coefficient INT  DEFAULT 1,
    yourbets VARCHAR(255),
    pricebet INT  DEFAULT 1
);
CREATE TABLE dice (
    ID BIGINT PRIMARY KEY,
    number INT DEFAULT 2,
    pricebet INT DEFAULT 1,
    diapazon VARCHAR(100)  DEFAULT 'Over',
    coeficient FLOAT  DEFAULT 1.07
);


3)Вскривши архів запускаємо .sln
4)Переходимо в пакети NuGet Відновлюємо завантаженні пакети ![image](https://github.com/user-attachments/assets/af2524fd-30a7-4d96-a6c5-fb9431830d16)
5)У класі Security - Copy.cs Забираємо символи коментарів та вписуємо свої: телеграм API(створюється у TG @BotFather), сервер де база даних MySQL, ім'я та пароль користувача MySQL
6)Видаляємо бібліотеку Security.cs та Solution -> Clean Solution і Solution -> Rebuild Solution.

Бот готовий до роботи
