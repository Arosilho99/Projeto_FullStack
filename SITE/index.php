<!DOCTYPE html>
<html>
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Exibição de Dados do Teste Técnico</title>
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bulma@1.0.0/css/bulma.min.css">
</head>
<body class="fundo">
<section class="section main-section">
  <div class="container ">
    <h1 class="title">
      Usuários cadastrados
    </h1>
    <div class="field">
      <label class="label">Pesquisar por nome/email:</label>
      <div class="control">
        <input class="input" type="text" id="searchInput" placeholder="">
      </div>
      <div class="column" id="resultado"></div>
    </div>
  </div>
</section>

<script>
  async function fetchData(id) {
    try {
      const response = await fetch(`http://localhost:3000/usuarios/${id}`);
      const data = await response.json();
      return data;
    } catch (error) {
      console.error('Erro ao obter os dados:', error);
    }
  }

  function calcularIdade(dataNascimento) {
    const hoje = new Date();
    const nascimento = new Date(dataNascimento);
    let idade = hoje.getFullYear() - nascimento.getFullYear();
    const mesAtual = hoje.getMonth() + 1;
    const mesNascimento = nascimento.getMonth() + 1;

    if (mesAtual < mesNascimento || (mesAtual === mesNascimento && hoje.getDate() < nascimento.getDate())) {
        idade--;
    }

    return idade;
  }

  async function showData(searchTerm = '') {
    const container = document.getElementById('resultado');
    container.innerHTML = ''; 

    let id = 1; 

    while (true) {
      try {
        const userData = await fetchData(id);

        if (!userData) {
          break;
        }

        if (
          searchTerm.trim() === '' || 
          userData.primeiro_nome.toLowerCase().includes(searchTerm.toLowerCase()) || 
          userData.ultimo_nome.toLowerCase().includes(searchTerm.toLowerCase())  ||
          userData.email.toLowerCase().includes(searchTerm.toLowerCase()) 
        ) {
          const idade = calcularIdade(userData.data_nascimento);

          const column = document.createElement('div');
          column.classList.add('column');
          column.innerHTML = `
          <div class="card cardBackground" >
            <div class="card-content">
              <div class="media">
                <div class="media-left">
                  <figure class="image is-48x48">
                    <img
                      src="${userData.foto_url}"
                      alt="Placeholder image"
                      class="is-rounded"
                    />
                  </figure>
                </div>
                <div class="media-content">
                  <p class="title is-4">${userData.primeiro_nome} ${userData.ultimo_nome}</p>
                  <p class="subtitle is-6"><p><strong></strong> <span class="email" onclick="copyEmail('${userData.email}')">${userData.email}</span></p></p>
                </div>
              </div>

              <div class="content">
                <p>Olá meu nome é ${userData.primeiro_nome} tenho ${idade} anos, meu país de origem é ${userData.pais}, meu telefone para contato é :</strong> <span class="telefone" onclick="copyTelefone('${userData.telefone}')">${userData.telefone}</span>
                <br />
              </div>
              </div>
          `;
          container.appendChild(column);
        }

        id++; 
      } catch (error) {
        console.error('Erro ao exibir os dados:', error);
        break; 
      }
    }
  }

  window.onload = () => {
    showData(); 
  };

  document.getElementById('searchInput').addEventListener('input', (event) => {
    showData(event.target.value);
  });

  function copyEmail(email) {
    const textarea = document.createElement('textarea');
    textarea.value = email;
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand('copy');
    document.body.removeChild(textarea);
    alert('O email foi copiado para a área de transferência.');
  }
  function copyTelefone(telefone) {
    const textarea = document.createElement('textarea');
    textarea.value = telefone;
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand('copy');
    document.body.removeChild(textarea);
    alert('O telefone foi copiado para a área de transferência.');
  }
</script>
</body>
<style>
  .usuarios{
    /* background-color: #222222; */
  }
  .fundo {
  background-image: url('img/wallpaper1.jpg');
  background-size: cover;
  background-position: center;
  background-attachment: fixed;
  color: white;
  min-height: 100vh;
}


  .main-section {
    min-height: 100vh; 
  }
  .fundo p{
   color: white; 
  }
  .fundo h1{
   color: white; 
  }
  .fundo label{
   color: white; 
  }
  .fundo strong {
      color: white; 
    }
    .email {
    cursor: pointer;
    color: #1E90FF; 
  }
  .telefone {
    cursor: pointer;
    color: #1E90FF; 
  }
  .cardBackground {
  background-color: #1C1C1C;
  box-shadow: 0px 0px 20px rgba(255, 255, 255, 0.8);
}
</style>
</html>
