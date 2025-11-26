import type { ReactElement } from "react";
import { Layout } from 'antd';
import packageJson from "../../../package.json";
import HeaderPageMenu from "../../shared/components/HeaderPageMenu";
import '../../App.css';

const { Header, Content, Footer } = Layout;

type ITemplate = {
  children: ReactElement;
};

const Template: React.FC<ITemplate> = ({ children }) => {
  return (
    <Layout className="template-layout">
      <Header className="template-header">
        <HeaderPageMenu />
      </Header>

      <Content className="template-content">
        <div className="template-content-container">
          {children}
        </div>
      </Content>

      <Footer className="template-footer">
        Gestão UPC @ V{packageJson.version}
      </Footer>
    </Layout>
  );
};

export default Template;