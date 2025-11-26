import { matchPath, useLocation } from "react-router-dom";
import RouterPaths from "./RouterPaths";
import { useEffect } from "react";

const routeTitles: Record<string, string> = {
  [RouterPaths.HOME]: 'Início'
}


export const RouterTitle = () => {
  const location = useLocation();

  useEffect(() => {
    let title = '';

    for (const path of Object.keys(routeTitles)) {
      const pathTitle = routeTitles[path];
      if (matchPath({ path, end: true }, location.pathname)) {
        title = pathTitle;
        break;
      }
    }

    document.title = title ? `Gestão UPC - ${title} ` : 'Gestão UPC';
  }, [location]);

  return null;
};
