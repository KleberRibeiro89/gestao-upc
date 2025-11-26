import React, { lazy, Suspense, type JSX } from "react";
import LoadingSpin from "../components/loading/LoadingSpin"

type Opts = {
  fallback: React.ReactNode;
}
type Unpromisify<T> = T extends Promise<infer P> ? P : never;

export const lazyLoad = <
  T extends Promise<any>,
  U extends React.ComponentType<any>
>(
  importFunc: () => T,
  selectorFunc?: (s: Unpromisify<T>) => U,
  opts: Opts = { fallback: <LoadingSpin /> }
) => {
  let lazyFactory: () => Promise<{ default: U }> = importFunc;

  if (selectorFunc) {
    lazyFactory = () =>
      importFunc().then((module) => ({ default: selectorFunc(module) }));
  }

  const LazyComponent = lazy(lazyFactory);

  return (props: React.ComponentProps<U>): JSX.Element => (
    <Suspense fallback={opts.fallback!}>
      <LazyComponent {...props} />
    </Suspense>
  );
};
