import { Link, useLocation } from 'react-router-dom';
import RouterPaths from '../../../core/router/RouterPaths';
import '../../../App.css';

const HeaderPageMenu = () => {
  const location = useLocation();

  const menuItems = [
    { label: 'Visão Geral', path: RouterPaths.VISAO_GERAL },
    { label: 'Lançamentos', path: RouterPaths.LANCAMENTOS },
    { label: 'Relatórios', path: RouterPaths.RELATORIOS },
  ];

  const isActive = (path: string) => {
    return location.pathname === path;
  };

  return (
    <div className="header-menu-container">
      <div className="flex items-center justify-between">
        <Link to={RouterPaths.HOME} className="flex items-center hover:opacity-80 transition-opacity">
          <img src="/logo.png" alt="Logo" style={{ width: '150px'}} />
        </Link>

        <nav className="header-menu-nav">
          {menuItems.map((item) => (
            <Link
              key={item.path}
              to={item.path}
              className={`px-4 py-2 rounded-md text-sm font-medium transition-colors ${
                isActive(item.path)
                  ? 'bg-white text-volcano-500'
                  : 'text-white hover:bg-volcano-400 hover:text-white'
              }`}
            >
              {item.label}
            </Link>
          ))}
        </nav>
      </div>
    </div>
  );
};

export default HeaderPageMenu;

