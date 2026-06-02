import {type FormEvent} from "react";
import {Plus, Search} from "lucide-react";

interface CustomerHeaderProps {
  searchEmail: string;
  onSearchChange: (value: string) => void;
  onSearchSubmit: (e: FormEvent) => void;
  onAddNew: () => void;
}

export function CustomerHeader({searchEmail, onSearchChange, onSearchSubmit, onAddNew}: CustomerHeaderProps) {
  return (
    <div className="flex justify-between items-center border-b border-gray-200 pb-4">
      <div>
        <h1 className="text-xl font-bold text-gray-800">Customers</h1>

        <p className="text-xs text-gray-400">
          Manage cafe clients, view profiles, and update bonus points.
        </p>
      </div>

      <div className="flex items-center gap-3">
        <form
          onSubmit={onSearchSubmit}
          className="flex items-center bg-white border border-gray-300 rounded-lg px-3 py-2
          shadow-sm focus-within:border-amber-500 transition-colors"
        >
          <Search className="w-4 h-4 text-gray-400 mr-2"/>

          <input
            type="email"
            placeholder="Find by email..."
            value={searchEmail}
            onChange={(e) => onSearchChange(e.target.value)}
            className="text-xs text-gray-700 placeholder-gray-400 focus:outline-none w-48"
          />
        </form>

        <button
          onClick={onAddNew}
          className="bg-amber-600 hover:bg-amber-700 text-white text-xs font-semibold px-4 py-2
          rounded-lg shadow-sm transition-colors flex items-center gap-1.5"
        >
          <Plus className="w-4 h-4"/>
          Add Customer
        </button>
      </div>
    </div>
  );
}