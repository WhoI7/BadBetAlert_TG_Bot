using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace CasinoInfo_bot
{
    internal class Acaunt
    {
        public long ID { get; set; }
        public string NN { get; set; }
        public string Stage { get; set; }
        public int C1 { get; set; }
        public int C5 { get; set; }
        public int C25 { get; set; }
        public int C50 { get; set; }
        public int C100 { get; set; }
        public int C500 { get; set; }
        public int C1000 { get; set; }
        public int Hands { get; set; }
        public int Win { get; set; }
        public int Lose { get; set; }
        public string Top { get; set; }

        string conect = Security.MySQL_Connection;

        public async Task SelectAsync(long UserId)
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                string comanda = $"SELECT * FROM acaunt WHERE ID = @ID;";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = UserId;

                    using (var read = await zapros.ExecuteReaderAsync())
                    {
                        read.Read();
                        this.ID = Convert.ToInt64(read["ID"]);
                        this.NN = Convert.ToString(read["NN"]);
                        this.Stage = Convert.ToString(read["Stage"]);
                        this.C1 = Convert.ToInt32(read["1"]);
                        this.C5 = Convert.ToInt32(read["5"]);
                        this.C25 = Convert.ToInt32(read["25"]);
                        this.C50 = Convert.ToInt32(read["50"]);
                        this.C100 = Convert.ToInt32(read["100"]);
                        this.C500 = Convert.ToInt32(read["500"]);
                        this.C1000 = Convert.ToInt32(read["1000"]);
                        this.Hands = Convert.ToInt32(read["Hands"]);
                        this.Win = Convert.ToInt32(read["Win"]);
                        this.Lose = Convert.ToInt32(read["Lose"]);
                        read.Close();
                    }
                }
            }
        }
        public async Task SelectStageAsync(long UserId)
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                string comanda = $"SELECT Stage FROM acaunt WHERE ID = @ID;";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = UserId;

                    using (var read = await zapros.ExecuteReaderAsync())
                    {
                        read.Read();

                        this.Stage = Convert.ToString(read["Stage"]);

                        read.Close();
                    }

                }

            }
        }
        public async Task SelectStageAndNNAsync(long UserId)
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                string comanda = $"SELECT Stage , NN FROM acaunt WHERE ID = @ID;";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = UserId;

                    using (var read = await zapros.ExecuteReaderAsync())
                    {
                        read.Read();

                        this.Stage = Convert.ToString(read["Stage"]);
                        this.NN = Convert.ToString(read["NN"]);

                        read.Close();
                    }

                }

            }
        }

        public async Task SelectNNAsync(long UserId)
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                string comanda = $"SELECT NN FROM acaunt WHERE ID = @ID;";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = UserId;

                    using (var read = await zapros.ExecuteReaderAsync())
                    {
                        read.Read();

                        this.NN = Convert.ToString(read["NN"]);

                        read.Close();
                    }

                }

            }
        }
        public async Task UpdateStageAsync(long UserId, string Stage)
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                string comanda = $"UPDATE acaunt SET Stage = @Stage where ID = @ID;";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@Stage", MySqlDbType.String).Value = Stage;
                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = UserId;
                    await zapros.ExecuteNonQueryAsync();

                }
            }
        }
        public async Task UpdateLose(long UserId)
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                string comanda = $"UPDATE acaunt SET Lose = Lose + 1, Hands = Hands + 1  where ID = @ID;";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = UserId;
                    await zapros.ExecuteNonQueryAsync();

                }

            }
        }
        public async Task Update21Win(long UserId)
        {
            Bet bet = new Bet();
            await bet.SelectAsync(UserId);
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                string comanda = $"UPDATE acaunt SET `1` = `1` + @C1, `5` = `5` + @C5, `25` = `25` + @C25, `50` = `50` + @C50, `100` = `100` + @C100, `500` = `500` + @C500, `1000` = `1000` + @C1000, Win = Win + 1, Hands = Hands + 1 WHERE ID = @ID;";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@C1", MySqlDbType.Int32).Value = bet.C1 * 2;
                    zapros.Parameters.Add("@C5", MySqlDbType.Int32).Value = bet.C5 * 2;
                    zapros.Parameters.Add("@C25", MySqlDbType.Int32).Value = bet.C25 * 2;
                    zapros.Parameters.Add("@C50", MySqlDbType.Int32).Value = bet.C50 * 2;
                    zapros.Parameters.Add("@C100", MySqlDbType.Int32).Value = bet.C100 * 2;
                    zapros.Parameters.Add("@C500", MySqlDbType.Int32).Value = bet.C500 * 2;
                    zapros.Parameters.Add("@C1000", MySqlDbType.Int32).Value = bet.C1000 * 2;
                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = UserId;

                    await zapros.ExecuteNonQueryAsync();
                }
            }


        }

        internal async Task TopPlayersBalancAsync()
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                string comanda = $"SELECT NN , (`1` + `5` * 5 + `25` * 25 + `50` * 50 + `100` * 100 + `500` * 500 + `1000` * 1000) AS balance FROM acaunt ORDER BY balance DESC LIMIT 10;";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    using (var read = await zapros.ExecuteReaderAsync())
                    {
                        this.Top = "🏆 *Топ гравців по балансу:*\n";
                       
                        while (read.Read()) 
                        {
                            this.Top += $"\n{read["NN"]} - {read["balance"]}";
                        }
                    }
                }
            }
        }
    }
}