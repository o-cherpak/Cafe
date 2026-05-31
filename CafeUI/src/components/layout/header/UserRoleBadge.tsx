interface UserRoleBadgeProps {
  role: string;
}

export function UserRoleBadge({role}: UserRoleBadgeProps) {
  return (
    <span className="text-xs bg-amber-100 text-amber-800 font-semibold px-2 py-1 rounded">
      {role}
    </span>
  );
}