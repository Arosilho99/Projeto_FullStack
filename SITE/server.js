const express = require('express');
const { Client } = require('pg');
const cors = require('cors');

const app = express();
const port = 3000;
app.use(cors());


// Configurações de conexão com o banco de dados
const client = new Client({
    user: 'postgres',
    host: 'localhost',
    database: 'postgres',
    password: '123456',
    port: 5432,
});

// Função para conectar ao banco de dados
async function connectToDatabase() {
    try {
        await client.connect();
        console.log('Conexão com o banco de dados estabelecida com sucesso!');
    } catch (error) {
        console.error('Erro ao conectar ao banco de dados:', error);
    }
}

// Rota para buscar dados de um usuário específico com base no ID
app.get('/usuarios/:id', async (req, res) => {
    const id = req.params.id;
    try {
        const resultado = await client.query('SELECT * FROM usuarios WHERE id = $1', [id]);
        if (resultado.rows.length > 0) {
            res.json(resultado.rows[0]);
        } else {
            res.status(404).send('Usuário não encontrado');
        }
    } catch (error) {
        console.error('Erro ao buscar usuário:', error);
        res.status(500).send('Erro ao buscar usuário');
    }
});

// Iniciar o servidor
app.listen(port, () => {
    console.log(`Servidor está ouvindo na porta ${port}`);
    connectToDatabase(); // Conectar ao banco de dados quando o servidor iniciar
});

// Exportar o cliente PostgreSQL para ser utilizado em outras partes do aplicativo
module.exports = client;
