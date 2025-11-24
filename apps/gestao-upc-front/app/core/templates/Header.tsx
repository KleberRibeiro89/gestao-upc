export function Header() {
  return (
    <>
      <header className="bg-amber-500 text-white p-4">
        <div className="container mx-auto">
          <h1 className="text-amber-100">Gestão UPC</h1>
        </div>
      </header>
      <div className="container mx-auto">
        <div className="flex flex-wrap">
          <div className="w-full md:w-1/2 p-4">
            <h2 className="text-amber-100">Sobre</h2>
            <p className="text-amber-100">Esta é uma aplicação web que permite gerir a gestão de um grupo de pessoas.</p>
          </div>
          <div className="w-full md:w-1/2 p-4">
            <h2 className="text-amber-100">Funcionalidades</h2>
            <ul className="text-amber-100">
              <li>Gerir o grupo de pessoas</li>
              <li>Gerir as tarefas</li>
              <li>Gerir os eventos</li>
            </ul>
          </div>
        </div>
      </div>
    </>
  );
}