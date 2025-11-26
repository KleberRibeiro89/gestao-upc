import type { ReactElement } from "react";
import Template from "../Template";

const withTemplate = (children: ReactElement) => {
    return <Template>{children}</Template>
}

export default withTemplate;
