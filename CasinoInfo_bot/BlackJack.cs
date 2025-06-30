using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace CasinoInfo_bot
{
    internal class BlackJack
    {
        public long ID { get; set; }
        public string card { get; set; }
        public string bankcard { get; set; }
        public int bankpoint { get; set; }
        public string playercard { get; set; }
        public int playerpoint { get; set; }
        

        string conect = Security.MySQL_Connection;

        public async Task SelectAsync(long UserId)
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                string comanda = $"SELECT * FROM 21game WHERE ID = @ID;";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = UserId;

                    using (var read = await zapros.ExecuteReaderAsync())
                    {
                        read.Read();

                        this.ID = Convert.ToInt64(read["id"]);
                        this.card = Convert.ToString(read["card"]);
                        this.bankcard = Convert.ToString(read["bankcard"]);
                        this.bankpoint = Convert.ToInt32(read["bankpoint"]);
                        this.playercard = Convert.ToString(read["playercard"]);
                        this.playerpoint = Convert.ToInt32(read["playerpoint"]);                        
                    }
                }
            }
        }
        public async Task UpdateGameAsync(long UserId)
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();
            
                string comanda = $"UPDATE 21game SET card = '{this.card}', bankcard = '{this.bankcard}', bankpoint = @bankpoint, playercard = '{this.playercard}', playerpoint = @playerpoint  WHERE ID = @ID";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@bankpoint", MySqlDbType.UInt32).Value = this.bankpoint;
                    zapros.Parameters.Add("@playerpoint", MySqlDbType.Int32).Value = this.playerpoint;
                    
                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = UserId;
                    await zapros.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task Reset21gameAsync(long UserId)
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                string updateCommand = @"
            UPDATE 21game 
            SET card = '', bankcard = '', bankpoint = 0, playercard = '', playerpoint = 0
            WHERE ID = @ID;";

                using (MySqlCommand command = new MySqlCommand(updateCommand, connection))
                {
                    command.Parameters.Add("@ID", MySqlDbType.Int64).Value = UserId;

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
