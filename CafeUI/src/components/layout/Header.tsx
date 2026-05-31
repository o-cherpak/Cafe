import {UserRoleBadge} from "./header/UserRoleBadge.tsx";
import {ApiStatusBadge} from "./header/ApiStatusBadge.tsx";
import {Logo} from "./header/Logo.tsx";

export default function Header() {
  const apiStatus: "connected" | "error" = "error";
  const userRole = "Admin";

  return (
    <header className="bg-white border-b border-gray-200 sticky top-0">
      <div className="flex gap-10 h-12 items-center justify-between px-10">
        <Logo/>

        <div className="flex items-center gap-4">
          <UserRoleBadge role={userRole}/>

          <ApiStatusBadge status={apiStatus}/>
        </div>
      </div>
    </header>
  );
}