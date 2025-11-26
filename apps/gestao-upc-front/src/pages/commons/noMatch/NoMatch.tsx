import React from 'react';

const NoMatch: React.FC = () => {
  return (
    <div className="min-h-full h-full bg-volcano-900 flex flex-col items-center justify-center text-white">
      <div className="text-center">
        <h1 className="text-9xl font-extrabold tracking-widest text-volcano-500">404</h1>
        <div className="bg-volcano-500 px-2 text-sm rounded rotate-12 absolute">
          Página não encontrada
        </div>
        <p className="mt-6 text-lg">Desculpe, a página que você procura não existe.</p>
        <a
          href="/"
          className="mt-8 inline-block rounded bg-volcano-500 px-6 py-2 text-sm font-medium text-white transition hover:bg-volcano-400"
        >
          Voltar para Home
        </a>
      </div>
    </div>
  );
};

export default NoMatch;
