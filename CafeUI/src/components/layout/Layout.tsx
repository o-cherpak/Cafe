import Header from "./Header";
import Footer from "./Footer";
import type {ReactNode} from "react";
import {Sidebar} from "./Sidebar.tsx";

interface LayoutProps {
  children: ReactNode;
}

export function Layout({children}: LayoutProps) {
  return (
    <div className="flex flex-col min-h-screen text-gray-900 bg-gray-50">
      <Header/>

      <main className="flex-grow w-full mx-auto px-6 py-8 flex gap-6">
        <Sidebar/>

        <div className="flex-grow bg-white rounded-xl border border-gray-200 p-6 shadow-sm">
          {children}
        </div>

      </main>

      <Footer/>
    </div>
  );
}