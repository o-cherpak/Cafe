type ApiStatusBadgeProps = { status: "connected" | "error" }

export function ApiStatusBadge({status}: ApiStatusBadgeProps) {
  const dotColor = status === "connected" ? "bg-green-500" : "bg-red-500";

  return (
    <div className="flex items-center gap-2 bg-gray-50 px-2 py-1 rounded-full border border-gray-200">
      <span className={`h-2 w-2 rounded-full ${dotColor}`}/>

      <span className="text-xs font-medium text-gray-600">API Status</span>
    </div>
  );
}
