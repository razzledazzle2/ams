import { Button } from "@/components/ui/button";
import { useNavigate } from "react-router-dom";
import { ArrowLeft, ChevronDown, LogOut, Plus } from "lucide-react";
import { logout } from "../utils/auth";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuPortal,
  DropdownMenuSeparator,
  DropdownMenuShortcut,
  DropdownMenuSub,
  DropdownMenuSubContent,
  DropdownMenuSubTrigger,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

export const NavigationBar = ({ onAdd, title }) => {
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const getUserFromToken = () => {
    const token = localStorage.getItem("accessToken");
    if (!token) return null;

    const payload = JSON.parse(atob(token.split(".")[1]));
    return {
      username: payload.unique_name,
      id: payload.uid,
    };
  };
  const user = getUserFromToken();
  console.log("User in NavBar:", user);
  return (
    <TooltipProvider>
      <header className="flex justify-between items-center p-4 bg-white">
        <h1 className="text-lg font-medium">{title}</h1>

        <div className="flex gap-1">
          <Tooltip>
            <TooltipTrigger asChild>
              <Button variant="ghost" size="icon" onClick={onAdd}>
                <Plus />
              </Button>
            </TooltipTrigger>
            <TooltipContent>Add asset</TooltipContent>
          </Tooltip>
          <DropdownMenu>
            <DropdownMenuTrigger className="flex items-center gap-2 text-sm">
              <span>{user?.username}</span>
              <ChevronDown size={14} />
            </DropdownMenuTrigger>

            <DropdownMenuContent align="end">
              <DropdownMenuItem onClick={handleLogout}>
                <LogOut className="mr-2" size={14} />
                Logout
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </header>
    </TooltipProvider>
  );
};
