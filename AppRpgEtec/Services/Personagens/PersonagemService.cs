﻿using AppRpgEtec.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRpgEtec.Services.Personagens
{
    public class PersonagemService : Request
    {
        private readonly Request _request;
        private const string apiUrlBase = "https://rpgapi20241pam.azurewebsites.net/Personagens";        
        //xyz --> site da sua API

        private string _token;
        public PersonagemService(string token)
        {
            _request = new Request();
            _token = token;
        }

        //Próximos métodos aqui
        public async Task<int> PostPersonagemAsync(Personagem p)
        {
            return await _request.PostReturnIntAsync(apiUrlBase, p, _token);
        }
        public async Task<ObservableCollection<Personagem>> GetPersonagensAsync()
        {
            string urlComplementar = string.Format("{0}", "/GetAll");
            ObservableCollection<Models.Personagem> listaPersonagens = await
      _request.GetAsync<ObservableCollection<Models.Personagem>>(apiUrlBase + urlComplementar, _token);
            return listaPersonagens;
        }
        public async Task<Personagem> GetPersonagemAsync(int personagemId)
        {
            string urlComplementar = string.Format("/{0}", personagemId);
            var personagem = await _request.GetAsync<Models.Personagem>(apiUrlBase + urlComplementar, _token);
            return personagem;
        }
        public async Task<int> PutPersonagemAsync(Personagem p)
        {
            var result = await _request.PutAsync(apiUrlBase, p, _token);
            return result;
        }
        public async Task<int> DeletePersonagemAsync(int personagemId)
        {
            string urlComplementar = string.Format("/{0}", personagemId);
            var result = await _request.DeleteAsync(apiUrlBase + urlComplementar, _token);
            return result;
        }

        public async Task<ObservableCollection<Personagem>> GetByNomeAproximadoAsync(string busca)
        {
            string urlComplementar = $"/GetByNomeAproximado/{busca}";

            ObservableCollection<Models.Personagem> listaPersonagens = await
      _request.GetAsync<ObservableCollection<Models.Personagem>>(apiUrlBase + urlComplementar, _token);

            return listaPersonagens;
        }

        public async Task<int> PutRestaurarPontosAsync(Personagem p)
        {
            string urlComplementar = "/RestaurarPontosVida";
            var result = await _request.PutAsync(apiUrlBase + urlComplementar, p, _token);
            return result;
        }

        public async Task<int> PutZerarRankingAsync(Personagem p)
        {
            string urlComplementar = "/ZerarRanking";
            var result = await _request.PutAsync(apiUrlBase + urlComplementar, p, _token);
            return result;
        }

        public async Task<int> PutZerarRankingRestaurarVidasGeralAsync()
        {
            string urlComplementar = "/ZerarRankingRestaurarVidas";
            var result = await _request.PutAsync(apiUrlBase + urlComplementar, new Personagem(), _token);
            return result;
        }



    }
}
